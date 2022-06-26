﻿using CsvHelper;
using CsvHelper.Configuration;
using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class TsvBackup
{
    // https://stackoverflow.com/questions/66166584/csvhelper-custom-delimiter

    CsvConfiguration importConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        //DetectDelimiter = true
        Delimiter = "\t",
        Quote = (char)1,
        Mode = CsvMode.NoEscape
    };

    CsvConfiguration exportConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t"
    };

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public TsvBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;
    }

    public async Task ImportData(Stream stream)
    {
        // https://joshclose.github.io/CsvHelper/examples/reading/get-anonymous-type-records/

        using (StreamReader streamReader = new(stream))
        {
            using (CsvReader csv = new CsvReader(streamReader, importConfig))
            {
                /*
                var records = new List<object>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new
                    {
                        Id = csv.GetField<int>("Id"),
                        Name = csv.GetField("Name"),
                        Tags = new List<string> { csv.GetField(5), csv.GetField(6), csv.GetField(7) }
                    };
                    records.Add(record);
                }
                /**/

                var anonymousTypeDefinition = new
                {
                    Root = string.Empty,
                    Category = string.Empty,
                    Goal = string.Empty,
                    Task = string.Empty,
                    Priority = Priority.None,
                    Interval = string.Empty
                };

                var records = csv.GetRecordsAsync(anonymousTypeDefinition);

                CategoryModel root;
                CategoryModel category;
                GoalModel goal;
                TaskModel task;

                await foreach (var record in records)
                {
                    if (_repository.CategoryList.Any(c => c.Name == record.Root))
                    {
                        root = _repository.CategoryList.First(c => c.Name == record.Root);
                    }
                    else
                    {
                        root = _repository.CreateCategory();
                        root.Name = record.Root;

                        await _repository.AddCategory(root);
                    }

                    if (root.CategoryList.Any(c => c.Name == record.Category))
                    {
                        category = root.CategoryList.First(c => c.Name == record.Category);
                    }
                    else
                    {
                        category = root.CreateCategory(_repository.MaxCategoryId + 1);
                        category.Name = record.Category;

                        await _repository.AddCategory(category);
                    }

                    if (category.GoalList.Any(g => g.Name == record.Goal))
                    {
                        goal = category.GoalList.First(g => g.Name == record.Goal);
                    }
                    else
                    {
                        goal = category.CreateGoal(_repository.MaxGoalId + 1);
                        goal.Name = record.Goal;

                        await _repository.AddGoal(goal);
                    }

                    task = goal.CreateTask(_repository.MaxTaskId + 1);
                    goal.Details += string.IsNullOrEmpty(goal.Details) ? record.Task : Environment.NewLine + record.Task;
                    task.Name = record.Task;
                    task.Priority = record.Priority;

                    string[] time = record.Interval.Split(' ');

                    if (time.Length == 2 && int.TryParse(time[0], out int interval))
                    {
                        task.DesiredTime = time[1].TrimEnd('s') switch
                        {
                            "hour" => TimeSpan.FromHours(interval),
                            "day" => TimeSpan.FromDays(interval),
                            "week" => TimeSpan.FromDays(interval * 7),
                            "month" => TimeSpan.FromDays(interval * 30),
                            "year" => TimeSpan.FromDays(interval * 365),
                            _ => TimeSpan.Zero
                        };
                    }

                    await _repository.AddTask(task);
                }
            }
        }

        //return data ?? throw new InvalidDataException("Can't deserialize TSV");
    }

    public async Task ExportData(IDataModel data)
    {
        // https://joshclose.github.io/CsvHelper/examples/writing/write-anonymous-type-objects/

        List<object> records = new List<object>();

        foreach (CategoryModel root in data.CategoryList)
        {
            foreach (CategoryModel category in root.CategoryList)
            {
                foreach (GoalModel goal in category.GoalList)
                {
                    foreach (TaskModel task in goal.TaskList)
                    {
                        records.Add(new { Root = root.Name, Category = category.Name, Goal = goal.Name, Task = task.Name, Priority = task.Priority, Interval = task.DesiredTime });
                    }
                }
            }
        }

        StringBuilder builder = new StringBuilder();

        using (StringWriter writer = new StringWriter(builder))
        {
            using (CsvWriter csv = new CsvWriter(writer, exportConfig))
            {
                csv.WriteRecords(records);
            }
        }

        string tsv = builder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.tsv", tsv);
    }
}