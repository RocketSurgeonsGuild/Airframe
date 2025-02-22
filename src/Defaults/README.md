# Airframe Defaults

When we create nullable reference type, we'd prefer to have a `default` but the keyword is usually just `null`.

## Requirements

- CSharp Language 12 or above (collection initializers)

## Goals

- [ ] Generate an Attribute to put on a class or record
  - [ ] Allow consumer to override the name of the property generated