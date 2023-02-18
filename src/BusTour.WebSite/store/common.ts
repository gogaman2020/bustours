import Vue from "vue"
import config from "@/config"

import { BaseResponse, RootState } from '~/types/booking'
import { SelectItem } from '~/types/common'
  
import { ActionTree, GetterTree, MutationTree } from 'vuex'

interface DomainEnums {
    [Key: string]: SelectItem[];
}
interface CommonState {
    domainEnums?: DomainEnums,
}

export const state = (): CommonState => ({
})

export const getters: GetterTree<CommonState, RootState> = {
    domainEnum: (state) => (enumName: string, t: any): SelectItem[] => {
        return state.domainEnums?.[enumName]?.map((x: SelectItem) => new SelectItem(x.value, t(`enums.${enumName}.${x.text}`))) ?? [];
    },
};

export const actions: ActionTree<CommonState, RootState> = {
    async getDomainEnums({ commit, state }) {
        if (!state.domainEnums) {
            try {
                commit('setDomainEnums', await this.$axios.$get<DomainEnums>(config.bustourApiUrl + '/Reference/GetDomainEnums'));
            }
            catch (error) {
                console.log(error);
            }
        }
    },
    async contactUs(_, request)  {
        await this.$axios.$post<BaseResponse>(config.bustourApiUrl + '/Jobs/contactUs', request);       
    }
};

export const mutations: MutationTree<CommonState> = {
    setDomainEnums(state: CommonState, data: DomainEnums[]) {
        Vue.set(state, 'domainEnums', data);
    },
};

export default {
    state,
    actions,
    mutations,
    getters
}