export default ({ app }) => {
    let redirect = app.$auth.$storage.options.redirect;
    for (let key in redirect) {
        redirect[key] = '/' + app.i18n.locale + redirect[key];
    }
    app.$auth.$storage.options.redirect = redirect;
}