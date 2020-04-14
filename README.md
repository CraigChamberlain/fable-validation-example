# Fable.Validation Example

[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io/#https://github.com/CraigChamberlain/fable-validation-example)

The [Fable.Validation](https://github.com/zaaack/fable-validation)
paket is used in this example to demonstrate a form validation and
styling in an Elmish workflow.

In particular see the validateModel function in src/app.fs.

It is modelled after [Zaid-Ajaj's](https://github.com/Zaid-Ajaj) [elmish-login-flow-validation](https://github.com/Zaid-Ajaj/elmish-login-flow-validation)
Which uses a previous version of Fable and other tools.

This project is built upon the [Fulma Minimal Template](https://fulma.github.io/Fulma/#template)

## How to use ?

### Architecture

- Entry point of your application is `src/App.fs`
- We are using [hmtl-webpack-plugin](https://github.com/jantimon/html-webpack-plugin) to make `src/index.html` the entry point of the website
- Entry point of your style is `src/scss/main.scss`
    - [Bulma](https://bulma.io/) and [Font Awesome](https://fontawesome.com/) are already included
    - We are supporting both `scss` and `sass` (by default we use `scss`)
- Static assets (favicon, images, etc.) should be placed in the `static` folder

### In development mode

*If you are using Windows replace `./fake.sh` by `fake.cmd`*

1. Run: `./fake.sh build -t Watch`
2. Go to [http://localhost:8080/](http://localhost:8080/)

*On Unix you may need to run `chmod a+x fake.sh`*

In development mode, we activate:

- [Hot Module Replacement](https://fable-elmish.github.io/hmr/), modify your code and see the change on the fly
- [Redux debugger](https://fable-elmish.github.io/debugger/), allow you to debug each message in your application using [Redux dev tool](https://github.com/reduxjs/redux-devtools)

### Build for production

*If you are using Windows replace `./fake.sh` by `fake.cmd`*

1. Run: `./fake.sh build`
2. All the files needed for deployment are under the `output` folder.
