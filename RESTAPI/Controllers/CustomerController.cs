using Microsoft.AspNetCore.Mvc;

namespace RESTAPI.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;



public class CustomersServer
{
    private List<Customer> customers;

    public CustomersServer()
    {
        customers = LoadCustomersFromFile();
    }

    public async Task StartAsync()
    {
        var host = new WebHostBuilder()
            .UseKestrel()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/customers", HandlePostCustomers);
                    endpoints.MapGet("/customers", HandleGetCustomers);
                });
            })
            .Build();

        await host.RunAsync();
    }

    private async Task HandlePostCustomers(HttpContext context)
    {
        try
        {
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var receivedCustomers = JsonConvert.DeserializeObject<List<Customer>>(requestBody);

            if (receivedCustomers == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var invalidCustomers = new List<Customer>();

            foreach (var customer in receivedCustomers)
            {
                if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.LastName)
                    || customer.Age <= 18 || IsIdUsed(customer.Id))
                {
                    invalidCustomers.Add(customer);
                }
            }

            if (invalidCustomers.Count > 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(invalidCustomers));
                return;
            }

            foreach (var customer in receivedCustomers)
            {
                int index = 0;
        while (index < customers.Count)
        {
            index++;
        }
                customers.Insert(index, customer);
            }

            SaveCustomersToFile();

            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }

    private async Task HandleGetCustomers(HttpContext context)
    {
        try
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(customers));
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }

    private bool IsIdUsed(int id)
    {
        foreach (var customer in customers)
        {
            if (customer.Id == id)
            {
                return true;
            }
        }
        return false;
    }


    private List<Customer> LoadCustomersFromFile()
    {
        // Load customers from a file (e.g., JSON, database) and return the list
        // If no persisted data is available, return an empty list
        // Example implementation:
        if (File.Exists("customers.json"))
        {
            var customersJson = File.ReadAllText("customers.json");
            if(customersJson.ToString() != string.Empty)
            {
            return JsonConvert.DeserializeObject<List<Customer>>(customersJson);
            }
        }
        return new List<Customer>();
    }

    private void SaveCustomersToFile()
    {
        var customersJson = JsonConvert.SerializeObject(customers);
        File.WriteAllText("customers.json", customersJson);
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var server = new CustomersServer();
        await server.StartAsync();
    }
}










