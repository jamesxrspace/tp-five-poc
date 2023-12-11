# Usecase Architecture

This package is aim to create facade and a template for usecase layer to simplify the implementation of use cases.

## Class diagram of usecase layer

[class diagram of usecase layer](./class.puml)

## Components

- `usecase/IFacade` is a facade of use cases, it should be put in the application layer. It is the entry to access use
  cases.
- `usecase/AbsFacade` is an abstract class of `IFacade`. It provides a template to execute the use cases.
- `usecase/IUsecase` defines the usecase interface. All usecase should implement it.
- `usecase/ICommand` defines the command interface. If you have a customized validator, you can implement it. Or the
  Abstract Facade will validate the command by the `validate` package.
- `usecase/example` is the example of how to implement the facade and usecase.

![class diagram of usecase layer](http://cdn-0.plantuml.com/plantuml/png/hLFDRlem3B_tAImkVv0etF-Q45ErIJp0TfsCwq6rvaE9aPY0Trz8QQXQBcFaa5QsVn-sbQKNTB9JTVPlAsBz__dSrgZFaHaPDQSQlS_B2ZSELHQEHVh23SDevvdGymWE6OI3QoyEII0Yu7djNv6mv9YDvsNFj1CUa-o1Z1RUownelbCWLGBgxmcCmpq5TiwuIO8UMlOJ58vH-Crp4SJ72RQwH2Ba5EygKBjF98RLuwpSev-Iy6S70-R2A8Mwl1A_OrsL69Wd3Uu2EtKsXzRM5Q5KHhVRQvOM391ZuJIkN_Ky84gG1xHzW_oUrgrnCkgslAdqnpQmrEmE7Qbk7F9yqHlDhPA-hxFkgYc7fsEUNvz4bluYBesn5svvigJT9eyBIBN3xA8__gnLQ7pqiqlMPNZkFm00)

## How to use

Please see [example](../example)
