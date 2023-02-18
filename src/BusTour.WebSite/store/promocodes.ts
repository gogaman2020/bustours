import config from "~/config";
import { GetterTree, ActionTree, MutationTree } from "vuex";
import { PromoCodeFilter, IPromocodeGridModel, Promocode, PromoCodeValidateFilter } from "@/types/promocodes";
import { DataRequest, IBadRequest, IDataResponse, Optional } from "~/types/common";

const apiUrl = config.bustourApiUrl;

interface IPromocodesState {
	promocode: Promocode;
	promoCodesGridFilter: PromoCodeFilter;
	promoCodesGridData: IPromocodeGridModel[]
}

export const state = (): IPromocodesState => ({
	promocode: new Promocode(),
	promoCodesGridFilter: new PromoCodeFilter(),
	promoCodesGridData: []
});

export const getters: GetterTree<IPromocodesState, IPromocodesState> = {
	promocode: (state: IPromocodesState): Promocode => state.promocode,
	promoCodesGridFilter: (state: IPromocodesState): PromoCodeFilter => state.promoCodesGridFilter,
	promoCodesGridData: (state: IPromocodesState): IPromocodeGridModel[] => state.promoCodesGridData
}

export const actions: ActionTree<IPromocodesState, IPromocodesState> = {
	async createPromocode({ state }): Promise<boolean | Optional<string>> {
		let response;

		try {
			response = await this.$axios.post<Promocode | IBadRequest>(`${apiUrl}/PromoCode/CreatePromoCode`, state.promocode);
			
			return true;
		} catch (error) {
			return (<IBadRequest>(<any>error)?.response?.data)?.message;
		}
	},
	async getAmountOfDiscount(): Promise<Array<number>> {
		const response = await this.$axios.get<Array<number>>(`${apiUrl}/PromoCode/GetAmountOfDiscount`);
		
		return response.data;
	},
	async getPromoCodesGridData({ getters, commit }): Promise<void> {
		const filter = new DataRequest(getters.promoCodesGridFilter);
		const response = await this.$axios.get<IDataResponse<IPromocodeGridModel>>(`${apiUrl}/PromoCode/GetPromoCodeList?request=${filter}`);

		commit("setPromoCodesGridData", response.data.items);
	},
	async updatePromoCode(_, payload: Promocode): Promise<void> {
		const response = await this.$axios.put(`${apiUrl}/PromoCode/UpdatePromoCode`, payload);
	},
	async getPromoCode({}, seriesNumber: string) {
        try {
            return await this.$axios.$get<Promocode>(config.bustourApiUrl + `/PromoCode/${seriesNumber}`);
        }
        catch (error) {
            console.log(error);
        }
    },
	async getPromoCodeWithValidation({}, filter: PromoCodeValidateFilter) {
        try {
            return await this.$axios.$post<Promocode>(config.bustourApiUrl + `/PromoCode/get-with-validate`, filter);
        }
        catch (error) {
            console.log(error);
        }
    },
}

export const mutations: MutationTree<IPromocodesState> = {
	setPromocode(state: IPromocodesState, payload: Promocode) {
		state.promocode = {...state.promocode, ...payload};
	},
	clearPromoCode(state: IPromocodesState): void {
		state.promocode = new Promocode();
	},
	setPromoCodesGridFilter(state: IPromocodesState, payload: PromoCodeFilter): void {
		state.promoCodesGridFilter = {...state.promoCodesGridFilter, ...payload};
	},
	setPromoCodesGridData(state: IPromocodesState, payload: IPromocodeGridModel[]): void {
		state.promoCodesGridData = payload;
	}
}

export default {
	state,
	getters,
	actions,
	mutations
}