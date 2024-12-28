# FORMS

![GitHub Main branch Status](https://img.shields.io/github/actions/workflow/status/candoumbe/forms/delivery.yml?branch=main&label=main)
![GitHub Develop branch Status](https://img.shields.io/github/actions/workflow/status/candoumbe/forms/integration.yml?branch=develop&label=develop)
[![Candoumbe.Forms](https://img.shields.io/nuget/vpre/candoumbe.forms?label=Candoumbe.Forms)](https://nuget.org/packages/Forms)
[![codecov](https://codecov.io/gh/candoumbe/forms/graph/badge.svg?token=N5IWOEWL80)](https://codecov.io/gh/candoumbe/forms)

## Introduction

This project aims at describing forms using HTML like syntax and objects.

The idea behind this project is to give opportunity to APIs written in .NET to describe their endpoints (responses and inputs) in a "HTML like way" so clients could **dynamically** generate inputs to push data to a given endpoints.

## Why

I know what you're thinking : "Open API and swagger already solve this !". Well, yes and no.
Yes, the [Open API] specification defines how to describe an API from a global perspective (shape of resources, supported HTTP verbs, ...).

But consider the following use case : you are building an API that handle bank accounts.

```json
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "object",
  "properties": {
    "id": {
      "type": "string",
      "format": "uuid"
    },
    "name": {
      "type": "string"
    },
    "balance": {
      "type": "number"
    },
    "currency": {
      "type": "string"
    }
  },
  "required": [
    "id",
    "balance",
    "currency",
    "name"
  ]
}
```

When an API client reads an account

```http http request
GET /accounts/{id}
```

the API would return an `account` with `balance` that is either :

- `balance > 0` :  you can withdraw money or make a deposit.
- `balance <= 0` :  you can only make a deposit.

How would you advertise that behavior to API consumers using only swagger ?
As far as I can tell, there's no way to do this as what can be done is very business/context/state specific.

With [Candoumbe.Forms], you have a set of classes to help you display that behavior when sending resources back to API clients.
Modeled after the [ION spec](https://ionspec.org), it provides a set of classes that can be used in API reponses to convey
additional meanings 

- [Form](./src/Forms/Form.cs) : models a form that a client must comply to in order to submit data to create a new account

Given the following `AccountModel`
```csharp
public record NewAccountModel(Guid Id, string Name, double InitialBalance);
```

the backend can "describe" the form for to use using the `FormBuilder` class as follow

```csharp
FormBuilder<NewAccountModel> createAccountFormBuilder = new(new Link { Href = "url/where/data/will/be/sent", Method = "POST", Relations = ["create-form"] });

createAccountFormBuilder.AddField(static x => x.Id);
createAccountFormBuilder.AddField(static x => x.Name);
createAccountFormBuilder.AddField(static x => x.InitialBalance);

Form form = createAccountFormBuilder.Build();
```

would result in the following `form` (JSON representation)

```json
{
  "fields": [
    {
      "label": "Id",
      "name": "Id"
    },
    {
      "label": "Name",
      "name": "Name"
    },
    {
      "type": "Integer",
      "label": "InitialBalance",
      "name": "InitialBalance"
    }
  ],
  "meta": {
    "href": "url/where/data/must/be/sent",
    "relations": [
      "create-form"
    ],
    "method": "POST",
    "template": false
  }
}
```

Given that json representation, any client could then build the form that could be used to create new accounts

using HTML

```html
<form action="url/where/data/must/be/sent">
    <label for="Id">Id</label>
    <input type="text" id="Id" name="Id"/>

    <label for="Name">Name</label>
    <input type="text" id="Name" name="Name"/>

    <label for="InitialBalance">Id</label>
    <input type="text" id="InitialBalance" name="InitialBalance"/>

    <button type="submit">Create account</button>
    <button type="button">Cancel</button>
</form>
```

Same could have been achieved with exploring an Open API ?!?!

You are definitely right ! That's exactly one of the purpose for which Open API was designed.

But [Candoumbe.Forms] is that you can add more metadatas when describing a [form](./src/Forms/Form.cs)

#### Going H.A.T.E.O.S.

One of the fundamental concept of HATEOS is to provide API responses that are self descriptive so that clients can discover the API without any
prior knowledge. 
For an `account` resource with positive balance

```json
{
  "item": {
    "id": "3bc34535dfde",
    "balance": 70,
    "currency": "$"
  },
  "links": [
    {
      "rel" : "deposit",
      "method": "POST",
      "href": "/accounts/3bc34535dfde/deposits"
    },
    {
      "rel" : "withdrawal",
      "method": "POST",
      "href": "/accounts/3bc34535dfde/withdrawals"
    }
  ]
}
```

whereas the following representation would be sent when `balance < 0`

```json
{
  "item": {
    "id": "3bc34535dfde",
    "balance": -20,
    "currency": "$"
  },
  "links": [
    {
      "rel" : "deposit",
      "verb": "POST",
      "link": "/accounts/3bc34535dfde/deposits"
    }
  ]
}
```

Providing consistent and dynamic navigation and/or action links allow any client to "react" and adapt to the state of the any resource.


[Candoumbe.Forms]: https://github.com/candoumbe/forms
[Open API]: https://swagger.io/specification/
[Form]: ./src/Forms/Form.cs
