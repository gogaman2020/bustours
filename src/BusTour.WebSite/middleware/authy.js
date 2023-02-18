import config from "@/config";
import { Roles, AuthorityLevel } from "@/types/private";

export default ({ store, route, redirect, error, app }) => {

    const disabled = route.meta.some(meta => !!meta?.auth?.disabled);

    if (disabled) {
        return;
    }

    const comingSoon = route.meta.some(meta => !!meta?.auth?.comingSoon);   

    if (comingSoon && config.showComingSoon) {
        redirect(app.localePath("/coming-soon"));
    }

    // Get authorizations for matched routes (with children routes too)
    const authorizationLevels = route.meta.map(meta => meta?.auth?.authority ?? null).filter(x => x !== null);

    if (!authorizationLevels?.length) {
        return;
    }

    //await app.$auth.fetchUser() 

    // Check if user is connected first
    if (!store.state.auth.loggedIn) {
        return config.showComingSoon ? redirect(app.localePath("/coming-soon")) : redirect(app.localePath("/authorization?returnUrl=" + (route?.fullPath ?? '')));
    }

    // Get highest authorization level
    const highestAuthority = Math.max.apply(null, authorizationLevels);

    let authority;
    switch (store.state.auth.user.role) {
        case Roles.Supervisor:
            authority = AuthorityLevel.Supervisor;
            break;
        case Roles.Administrator:
            authority = AuthorityLevel.Administrator;
            break;
        case Roles.Crew:
            authority = AuthorityLevel.Crew;
            break;
        default:
            authority = AuthorityLevel.User;
            break;
    }
    if (authority < highestAuthority) {
        return config.showComingSoon
            ? redirect(app.localePath("/coming-soon"))
            : error({
                statusCode: 401,
                message: 'You do not have permission to access this page'
            });
    }
};