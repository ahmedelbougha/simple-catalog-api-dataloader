using System;
using System.Threading.Tasks;
using aspnetcoregraphql.Data;
using aspnetcoregraphql.Models.Operations;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace aspnetcoregraphql.Controllers
{
    [Route("graphql")]
    public class GraphQLController : Controller
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly DataLoaderDocumentListener _listener;
        private readonly IServiceProvider _services;
        public GraphQLController(IDocumentExecuter documentExecuter,ISchema schema, DataLoaderDocumentListener listner, IServiceProvider services)
        {
            _documentExecuter = documentExecuter;
            _schema = schema;
            // _listener = listner;
            _services = services;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            
            var complexityConfiguration = new ComplexityConfiguration {
                // MaxDepth = 1,
                // MaxComplexity = 85,
                // FieldImpact = 5.0             
            };
            var executionOptions = new ExecutionOptions { 
                Schema = _schema, 
                Query = query.Query,
                Inputs = query.Variables.ToInputs(),
                ComplexityConfiguration = complexityConfiguration,
                ExposeExceptions = true,
                
            };
            executionOptions.Listeners.Add((IDocumentExecutionListener)_services.GetRequiredService(typeof(DataLoaderDocumentListener)));

            try
            {
                var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

                if (result.Errors?.Count > 0)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}