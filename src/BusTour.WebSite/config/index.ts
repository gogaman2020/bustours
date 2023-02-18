export default {
    bustourSiteUrl: process.env.NUXT_ENV_BUSTOUR_SITE_URL as string,
    bustourApiUrl: process.server && process.env.NUXT_ENV_SSR_API_URL ? process.env.NUXT_ENV_SSR_API_URL : process.env.NUXT_ENV_BUSTOUR_API_URL,
    maxGuestCount: parseInt(process.env.NUXT_ENV_MAX_GUEST_COUNT || ""),
    maxDisabledGuestCount: parseInt(process.env.NUXT_ENV_MAX_DISABLED_GUEST_COUNT || ""),
    moreThanMaxGuestCountValue: 100,
    serviceCharge: parseFloat(process.env.NUXT_ENV_SERVICE_CHARGE || ""),
    vat: parseInt(process.env.NUXT_ENV_VAT || ""),
    maxYear: 10,
    strictMode: (process.env.NUXT_ENV_STRICT_MODE == "true"),
    showComingSoon: (process.env.NUXT_ENV_SHOW_COMING_SOON == "true"),
    supportEmail: process.env.NUXT_ENV_SUPPORT_EMAIL as string,
    formats: {
        date: {
            short: 'DD.MM.YYYY',
            long: 'DD MMMM YYYY'
        },
        time: {
            short: "HH:mm"
        },
        datetime: {
            short: "DD.MM.YYYY HH:mm"
        }
    }
}