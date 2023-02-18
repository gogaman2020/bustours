import Vue from "vue"
import config from "@/config";
import { ActionTree, GetterTree, MutationTree } from 'vuex'
import { GiftCertificateAdding, AmountVariant, GiftCertificate, GiftCertificateStatusTotals } from '@/types/giftCertificate'
import { Payment } from '@/types/booking'
import { Certificate } from "crypto";

interface GiftCertificateState {
    certificateAdding: GiftCertificateAdding;
    amountVariants: AmountVariant[];
}

export const state = (): GiftCertificateState => ({
    certificateAdding: new GiftCertificateAdding(),
    amountVariants: []
})

export const getters: GetterTree<GiftCertificateState, GiftCertificateState> = {
};

export const actions: ActionTree<GiftCertificateState, GiftCertificateState> = {
    async getGiftCertificate({ commit }, id) {
        try {
            let certificate = await this.$axios.$get<GiftCertificate>(config.bustourApiUrl + `/GiftCertificate/${id}`);
            return certificate;
        }
        catch (error) {
            console.log(error);
        }
    },         
    async getGiftCertificates({ commit }, filter) {
        try {
            return await this.$axios.$get<GiftCertificate[]>(config.bustourApiUrl + `/GiftCertificate/Filter`, 
            { 
                params: { filter: filter },
            });
        }
        catch (error) {
            console.log(error);
        }
    },      
    async getGiftCertificateStatusesTotals({ commit }) {
        try {
            return await this.$axios.$get<GiftCertificateStatusTotals[]>(config.bustourApiUrl + `/GiftCertificate/GetStatusesTotals`);
        }
        catch (error) {
            console.log(error);
        }
    },       
    async getAmountVariants({ commit }) {
        try {
            commit('setAmountVariants', await this.$axios.$get<AmountVariant[]>(config.bustourApiUrl + '/GiftCertificate/GetAmountVariants'));
        }
        catch (error) {
            console.log(error);
        }
    },       
    async postCertificateAdding({ state, commit }) {
        try {
            var response = await this.$axios.$post<GiftCertificate>(config.bustourApiUrl + '/GiftCertificate/AddCertificate', 
            {...state.certificateAdding, ...{ id: state.certificateAdding?.addedCertificate?.id ?? 0 }}
            );
            commit('setCertificateAdding', { addedCertificate: response });
            return response;
        }
        catch (error) {
          console.log(error);
        }
    },
    async sendCertificatePaymentSuccess({ state, commit }) {
        try {
            var response = await this.$axios.$post<Payment>(config.bustourApiUrl + '/Payment/CertificatePaymentSuccess', {
                certificateId: state.certificateAdding.addedCertificate.id,
            });
            return response;
        }
        catch (error) {
            console.log(error);
        }
    },		
    async sendCertificatePaymentFail({ state, commit }, error: string) {
        try {
            var response = await this.$axios.$post<Payment>(config.bustourApiUrl + '/Payment/CertificatePaymentFail', {
                certificateId: state.certificateAdding.addedCertificate.id,
                error: error
            });
            return response;
        }
        catch (error) {
            console.log(error);
        }
    },	
    async sendCertificateOnEmail({ state, commit }, email) {
        try {
            var response = await this.$axios.$post<GiftCertificate>(config.bustourApiUrl + '/GiftCertificate/SendCertificateOnEmail', 
            { ...{ id: state.certificateAdding?.addedCertificate?.id, email: email}}
            );
            return response;
        }
        catch (error) {
          console.log(error);
        }
    },
    async getCertificatePdf(_, certificateId) {
        try {
            const response = await this.$axios.$get<string>(
                `${config.bustourApiUrl}/GiftCertificate/GetCertificatePdf?certificateId=${certificateId}`
            );

            return response;
        } catch (error) {
            console.error(error);
        }
    },
};

export const mutations: MutationTree<GiftCertificateState> = {
    setAmountVariants(state: GiftCertificateState, data: AmountVariant[]) {
        Vue.set(state, 'amountVariants', data);
    },
    setCertificateAdding(state: GiftCertificateState, data: GiftCertificateAdding) {
        state.certificateAdding = {...state.certificateAdding, ...data};
    },
    resetAddedCertificate(state: GiftCertificateState) {
        state.certificateAdding.addedCertificate = {...state.certificateAdding.addedCertificate, ...new GiftCertificate()};
    },
    setAddedCertificate(state: GiftCertificateState, data: GiftCertificate) {
        state.certificateAdding.addedCertificate = {...state.certificateAdding.addedCertificate, ...data};
    }    
};

export default {
    state,
    actions,
    mutations,
    getters
}