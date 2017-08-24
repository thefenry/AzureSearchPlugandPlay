# AzureSearchPlugandPlay
Simple Library to plug and play with Azure Search

## Setup
- Add the class library to your solution
- Restore the nuget packages
- Add your SearchServiceName and Key to the webconfig (*Make sure to match the name as listed below*)

```
    <add key="SearchServiceName" value="XXXXXXXX" />
    <add key="SearchServiceAdminApiKey" value="XXXXXXXX" />
```

## How to use

### Create a model

Create a model and add the azure search annotation. (**note** *This may require adding an azure search reference to your models project*)

```
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Search;

    namespace Azure_Search_and_Recommendations_Demo.Models
    {
        public class Car
        {
            [Key]
            public int Id { get; set; }

            [IsSearchable, IsFilterable, IsSortable]
            public string Make { get; set; }

            [IsSearchable, IsFilterable, IsSortable, IsFacetable]
            public string Model { get; set; }

            [IsFilterable, IsSortable, IsFacetable]
            public int Year { get; set; }

            [IsFilterable, IsSortable, IsFacetable]
            public int Rating { get; set; }

        }
    }
```

### Initialize the Library Service
Pass the index name when newing up the instance of the index
```
 AzureSearchManager _azureSearch = new AzureSearchManager(indexName)
```

### Create an Index

Call the library's CreateIndex method of Type Model and pass the index name as a parameter.
```
    azureSearchManager.CreateIndex<Car>("cars");
```

### Populate Index with a batch 

Call the Library's AddOrUpdateDocumentToIndex method and pass a list of object of the type you used to create the index

```
    List<Car> cars = new List<Car>();
    azureSearchManager.AddOrUpdateDocumentToIndex(cars);
```

### Delete item from Index

Generate a list of object to delete from the index

```
 List<Car> cars = new List<Car>();
 azureSearchManager.DeleteDocumentInIndex(cars);
```

### Get all documents from Azure Search
Call the SearchContent Method by assigning the Object Type as below.

```
return azureSearchManager.SearchContent<Car>().Results;
```

### Query and/or Facet search content in the Index
Call the SearchContent Method by assigning the Object Type as below and passing the search term and/or . 

```
return azureSearchManager.SearchContent<Car>(searchTerm).Results;
```

You can also add a facets to your search

```
List<string> facets = new List<string>{"Year", "Model,count:12"}

return azureSearchManager.SearchContent<Car>(searchTerm, facets).Results;
```

### Filter and/or Facet search content in the Index
Call the FilterContent Method by assigning the Object Type as below and passing the filter string term and/or pass the optional facets. 

```
List<string> facets = new List<string>{"Year", "Model,count:12"}

return azureSearchManager.FilterContent<Car>(filter, facets).Results;
```