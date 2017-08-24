using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace AzureSearchPlugAndPlay
{
    public class AzureSearchManager
    {
        private readonly SearchServiceClient _searchClient;
        private SearchIndexClient _indexClient;

        private string SearchServiceName
        {
            get { return ConfigurationManager.AppSettings["SearchServiceName"]; }
        }

        public string AdminApiKey
        {
            get { return ConfigurationManager.AppSettings["SearchServiceAdminApiKey"]; }
        }

        public AzureSearchManager(string indexName)
        {
            _searchClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(AdminApiKey));
            _indexClient = GetIndexClient(indexName);
        }

        public void CreateIndex<T>(string indexName)
        {
            if (_searchClient.Indexes.Exists(indexName))
            {
                return;
            }

            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<T>()
            };

            Field field = definition.Fields.FirstOrDefault(x => x.IsKey && x.Type != DataType.String);
            field.Type = DataType.String;


            _searchClient.Indexes.Create(definition);
        }


        public void CreateIndex(Index definition)
        {
            _searchClient.Indexes.Create(definition);
        }


        public DocumentIndexResult AddOrUpdateDocumentToIndex<T>(List<T> documents) where T : class, new()
        {
            IndexBatch<T> batch = IndexBatch.Upload(documents);

            try
            {
                return _indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                throw e;
            }
        }

        public DocumentIndexResult DeleteDocumentInIndex<T>(List<T> documents) where T : class, new()
        {
            IndexBatch<T> batch = IndexBatch.Delete(documents);

            try
            {
                return _indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                throw e;
            }
        }

        public DocumentSearchResult<T> SearchContent<T>(string searchTerm = "*", List<string> facets = null) where T : class, new()
        {
            SearchParameters searchParams = SetSearchParams(string.Empty, facets);

            return _indexClient.Documents.Search<T>(searchTerm, searchParams);
        }

        public DocumentSearchResult<T> FilterContent<T>(string filter, List<string> facets = null) where T : class, new()
        {
            SearchParameters parameters = SetSearchParams(filter, facets);

            return _indexClient.Documents.Search<T>("*", parameters);
        }

        private SearchIndexClient GetIndexClient(string indexName)
        {
            return new SearchIndexClient(SearchServiceName, indexName, new SearchCredentials(AdminApiKey));
        }

        private SearchParameters SetSearchParams(string filter, List<string> facets)
        {
            return new SearchParameters()
            {
                Facets = facets,
                Filter = filter
            };
        }

    }
}
