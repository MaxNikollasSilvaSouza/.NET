using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Dotnet.Models;

namespace Company.Function
{
    public static class main
    {
        [FunctionName("Produtos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
           try
           {
               var client = new MongoClient(
                "mongodb+srv://Username:password@restapi.5pflo.mongodb.net/databasename?retryWrites=true&w=majority"
                );
                var database = client.GetDatabase("databasename");
                 IMongoCollection<Product> products = database.GetCollection<Product>("colecao");

               
                if (req.Method == "POST")
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    var requestJson = JsonConvert.DeserializeObject<Product>(requestBody);

                    if (!((requestJson.NOME is string) && (requestJson.DESCRICAO is string)))
                    {
                        log.LogWarning("Bad Request");
                        return new BadRequestResult();
                    }

                    Product product = new Product
                    {
                        NOME = requestJson.NOME,
                        DESCRICAO = requestJson.DESCRICAO,
                        VALOR = requestJson.VALOR
                    };

                    products.InsertOne(product);
                    return new OkResult();
                }

                else if (req.Method == "GET")
                {
                    var response = products.Find(product => true).ToList();
                    if (response == null)
                    {
                        log.LogInformation($"Products not found");
                        return new NotFoundResult();
                    }
                    else
                    {
                        return new OkObjectResult(response);
                    }
                }

                else if (req.Method == "DELETE")
                {
					var deletedProduct = products.DeleteOne(product => product.NOME == req.Query["nome"]);

					if (deletedProduct.DeletedCount == 0)
					{
						log.LogInformation("Product Not Found");
						return new NotFoundResult();
					}

					return new OkResult();
                }
               
               else if (req.Method == "PUT")
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    var requestJson = JsonConvert.DeserializeObject<Product>(requestBody);

                    Product updatedProduct = new Product
                    {
                        _id = requestJson._id,
                        NOME = requestJson.NOME,
                        DESCRICAO = requestJson.DESCRICAO,
                        VALOR = requestJson.VALOR
                    };

                    var replacedProduct = products.ReplaceOne(product => product.NOME == requestJson.NOME, updatedProduct);

                    if (replacedProduct.MatchedCount == 0)
                    {
                        log.LogInformation("Product Not Found");
                        return new NotFoundResult();
                    }
                    else
                    {
                        return new OkObjectResult("OK");
                    }

                }

                else
                {
                    return new BadRequestResult();
                }


           
           }

           catch (Exception e)
           {
               log.LogError($"Exception thrown: {e.Message}");
               return new StatusCodeResult(StatusCodes.Status500InternalServerError);
           }
        }
    }
}
