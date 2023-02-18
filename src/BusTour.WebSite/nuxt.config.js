import logger from "connect-logger";

export default {
    // ssr: false,
    loading: {
        duration: 1000,
        continuous: true
    },
    router: {
        extendRoutes(routes, resolve) {                         
        }
    },  
  server: {
    port: 3002, // default: 3000
    //host: '0.0.0.0' // default: localhost
  },
  /*
  ** Nuxt rendering mode
  ** See https://nuxtjs.org/api/configuration-mode
  */
  mode: 'universal',
  /*
  ** Nuxt target
  ** See https://nuxtjs.org/api/configuration-target
  */
  target: 'server',
  /*
  ** Headers of the page
  ** See https://nuxtjs.org/api/configuration-head
  */
  head: {
    title: process.env.npm_package_name || '',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: process.env.npm_package_description || '' }
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
    ]
  },
  /*
  ** Global CSS
  */
  css: [],
  /*
  ** Plugins to load before mounting the App
  ** https://nuxtjs.org/guide/plugins
  */
  plugins: [
    { src: './plugins/vue-awesome-swiper.ts', ssr: true },
  ],
  /*
  ** Auto import components
  ** See https://nuxtjs.org/api/configuration-components
  */
  components: true,
  /*
  ** Nuxt.js dev-modules
  */
  buildModules: [
    '@nuxt/typescript-build',
    '@nuxtjs/vuetify'
  ],
  /*
  ** Nuxt.js modules
  */
  modules: [
    '@nuxtjs/axios',
    '@nuxtjs/auth',
    '@nuxtjs/dotenv',
    'nuxt-i18n',
  ],
  /*
   ** Axios module configuration
   ** See https://axios.nuxtjs.org/options
   */
  axios: {
    baseURL: process.env.NUXT_ENV_BUSTOUR_API_URL
  },
  /*
  ** Build configuration
  ** See https://nuxtjs.org/api/configuration-build/
  */
  build: {
    loaders: {
      css: {
        modules: {
          auto: true,
          localIdentName: '[local]-[hash:3]'
        },
        localsConvention: 'camelCaseOnly'
      },
    },
    extractCSS: process.env.NUXT_ENV_EXTRACT_CSS ?? false,
  },

  serverMiddleware: [
    logger({ format: "%date %status %method %url (%time)" })
  ],

  i18n: {
    locales: [
      {
        name: 'English',
        code: 'en',
        iso: 'en-US',
        file: 'en.js'
      },
      {
        name: 'Français',
        code: 'fr',
        iso: 'fr-FR',
        file: 'fr.js'
      },
      {
        name: 'Español',
        code: 'es',
        iso: 'es-ES',
        file: 'es.js',
      },
      // {
      //   name: '中文',
      //   code: 'zh (中文)',
      //   iso: 'zh-CN',
      //   file: 'zh.js',
      // },
      {
        name: 'Русский',
        code: 'ru',
        iso: 'ru-RU',
        file: 'ru.js'
      },
    ],
    defaultLocale: 'en',
    detectBrowserLanguage: false,
    strategy: 'prefix_except_default',
    lazy: true,
    langDir: 'lang/',
    vuex: {
      moduleName: 'i18n',
      syncLocale: true,
    },
    vueI18n: {
      fallbackLocale: 'en',
    }
  },

  auth: {
    strategies: {
      local: {
        endpoints: {
          login: {
            url: '/auth/authenticate',
            method: 'post'
          },
          logout: false,
          user: {
            url: '/user/current',
            method: 'get',
            propertyName: false
          }
        }
      }
    },
    plugins: [
      "@/plugins/vuelidate.js",
      "@/plugins/global-components.ts",
      "@/plugins/array-extensions.ts",
    ],
  },
}