import config from "@/config";
export const strict = config.strictMode;

import { ActionContext, ActionTree } from 'vuex'
import { RootState } from '../types/booking'

interface Actions<S, R> extends ActionTree<S, R> {
  nuxtServerInit(actionContext: ActionContext<S, R>): void
}

export const actions: Actions<RootState, RootState> = {
  async nuxtServerInit({ dispatch }) {
    await Promise.all([
      dispatch('booking/getLanguages'),
      dispatch('booking/getMenuInfo'),
      dispatch('booking/getRouteInfo'),
      dispatch('common/getDomainEnums'),
      dispatch('booking/getBuses'),
      dispatch('tour/getBuses'),
      dispatch('tour/getCities'),
      dispatch('tour/getRoutes')
    ]);
    // await dispatch('booking/getLanguages');
    // await dispatch('booking/getMenuInfo');
    // await dispatch('booking/getRouteInfo');
    // await dispatch('common/getDomainEnums');
    // await dispatch('booking/getBuses');
    // await dispatch('tour/getBuses');
    // await dispatch('tour/getCities');
    // await dispatch('tour/getRoutes');
  },
}