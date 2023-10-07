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
      "type": "uuid"
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
    "currency"
  ]
}
```

When an API client reads an account

```http
GET /accounts/{id}
```

the API would return an `account` with `balance` that is either :

- `balance > 0` :  you can withdraw money or make a deposit.
- `balance <= 0` :  you can only make a deposit.

How would you advertise that behavior to API consumers using only swagger ?
As far as I can tell, there's no way to do this as what can be done is very business/context/state specific

With [Candoumbe.Forms], you have a set of classes to help you display that behavior when sending resources back to clients. Modeled after the ION spec

- [Form] : models a form that can must be submitted

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
      "verb": "POST",
      "href": "/accounts/3bc34535dfde/deposits"
    },
    {
      "rel" : "withdrawal",
      "verb": "POST",
      "href": "/accounts/3bc34535dfde/withdrawals"
    }
  ]
}
```

whereas the following representation will be sent when `account.balance < 0`

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

[Candoumbe.Forms]: https://github.com/candoumbe/forms
[Open API]: https://swagger.io/specification/
[Form]: ./src/Forms/Form.cs
[LinkRelation]: ./src/Forms/LinkRelation.cs
