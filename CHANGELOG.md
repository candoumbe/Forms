# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### New features
- Changed `Link.Relation` from `string` to `string[]` [BREAKING]

### Breaking changes
- Removed `Ultimately` dependency
- Changed `IonResource`to a record (`netstandard2.1`+ and `net5`+)
- Changed `Form`to a record (`netstandard2.1`+ and `net5`+)
- Changed `FormField`to a record (`netstandard2.1`+ and `net5`+)
- Changed `Link`to a record (`netstandard2.1`+ and `net5`+)

## [0.2.1] / 2021-01-18
### New Features
- Added [`FormFieldOption`](/src/Forms/FormFieldOption.cs)

### Fixes
- Fixed failing build pipeline

## [0.1.0]
- Initial release

