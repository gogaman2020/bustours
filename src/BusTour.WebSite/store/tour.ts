import Vue from "vue"
import config from "@/config";
import { ActionTree, GetterTree, MutationTree } from 'vuex'

import { Route, Bus, Table, Seat, BaseResponse, SeatType } from '@/types/booking'
import { ToursCreation, TourCreation, City, Tour, TourFB, TourMenuBeverage, ITourInformation, CurrentBooking } from '@/types/tour'
import { Dictionary } from '@/types/common'

interface TourState {
    toursCreation: ToursCreation;
    tourMenuBeverage: TourMenuBeverage;
    cities: City[];
    routes: Route[];
    tour: Tour;
    tourFB: TourFB;
    tourInfo: ITourInformation;
    currentBookings: CurrentBooking[];
    buses: Bus[];
}

export const state = (): TourState => ({
    toursCreation: {
        unavailableDates: [],
        tours: []
    },
    cities: [],
    routes: [],    
    tourMenuBeverage: {
        id: 0,
        tourMenus: [],
        tourBeverages: []
    },
    tour: {
        id: 0,
        departure: undefined,
        city: undefined,
        tourType: 0,
        itinerary: undefined,
        status: "",
        duration: undefined
    },
    tourFB: {
        toursInfo:[],
        menus:[],
        beverages:[],
        extraMenus:[],
        extraBeverages:[],
        privateTourInfo:[]
    },
    tourInfo: {} as ITourInformation,
    currentBookings:[],
    buses: []
})

export const getters: GetterTree<TourState, TourState> = {
    tourInformation: (state: TourState) => state.tourInfo,
    cities: (state: TourState) => state.cities,
    seatFullName: (state, getters) => (seatId: number): string => {

        let resultTable: Table | null = null
        let resultSeat: Seat | null = null

        state.buses.forEach(bus => {
            bus.tables.forEach(table => {
                table.seats.forEach(seat => {
                    if (seat.id == seatId) {
                        resultTable = table as Table
                        resultSeat = seat as Seat
                    }
                })
            })
        })

        if (resultTable && resultSeat) {
            if ((resultSeat as Seat).type == SeatType.Disabled) {
                return `Wheel chair`
            }
            else {
                return `${(resultTable as Table).number} ${(resultSeat as Seat).name}`
            }
        } else {
            return ''
        }
    },    
};

export const actions: ActionTree<TourState, TourState> = {
    async createTours({ state }) {
        try {
          return await this.$axios.$post<number>(config.bustourApiUrl + '/Tour/create-tours', state.toursCreation)
        }
        catch (error) {
          throw (error as any).response.data
        }
    },
    async getCities({ state, commit }) {
        if (!state.cities?.length) {
            try {
                var response = await this.$axios.$get<City[]>(config.bustourApiUrl + '/Tour/GetCities');
                commit('setCities', response);
            }
            catch (error) {
                console.log(error);
            }
        } else {
            return state.cities
        }
    },
    async getRoutes({ state, commit }) {
        if (!state.routes?.length) {
            try {
                var response = await this.$axios.$get<Route[]>(config.bustourApiUrl + '/Tour/GetRoutes');
                commit('setRoutes', response);
            }
            catch (error) {
                console.log(error);
            }
        } else {
            return state.routes
        }
    },    
    async getBuses({ state, commit }) {
        if (!state.buses?.length) {
            try {
                commit('setBuses', await this.$axios.$get<Route[]>(config.bustourApiUrl + '/Tour/GetBuses'));
            }
            catch (error) {
                console.log(error);
            }
        } 
        return state.buses
    },      
    async filter({ state }, filter) {
        try {
            return await this.$axios.$get<Dictionary<Tour[]>>(config.bustourApiUrl + `/Tour/Filter`, 
            { 
                params: { filter: filter },
            });            
        }
        catch (error) {
            console.log(error);
        }        
    },
    async deleteTours({state}, ids) {
        try {
            return await this.$axios.$post<number[]>(config.bustourApiUrl + `/Tour/Delete`, ids);            
        }
        catch (error) {
            console.log(error);
        }             
    },
    async getTourMenuBeverage({ commit }, id) {
        try {
            let tourMenuBeverage = await this.$axios.$get<TourMenuBeverage>(config.bustourApiUrl + `/Tour/get-menu-beverage/${id}`);
            commit('setTourMenuBeverage', tourMenuBeverage); 
        }
        catch (error) {
            console.log(error);
        }
    },
    async getTourById({ commit }, id) {
        try {
            let tour = await this.$axios.$get<Tour>(config.bustourApiUrl + `/Tour/get-tour/${id}`);
            commit('setTourById', tour); 
        }
        catch (error) {
            console.log(error);
        }
    }, 
    async getTourFB({ commit }, filter) {
        try {
            let tours = await this.$axios.$get<TourFB>(config.bustourApiUrl + `/Tour/get-tour-fb`,{ 
                params: { filter: filter },
            });
            commit('setTourFB', tours); 
        }
        catch (error) {
            console.log(error);
        }
    },
    async getTourInformation({ commit }, tourId: number): Promise<void> {
        try {
            const tourInfo = await this.$axios.get<ITourInformation>(`${config.bustourApiUrl}/Tour/GetTourInformation/${tourId}`);

            commit("setTourInformation", tourInfo.data);
        } catch (error) {
            console.error(error);
        }
    },
    async currentBookings({ commit }, filter) {
        try {
            let tours = await this.$axios.$get<CurrentBooking[]>(config.bustourApiUrl + `/Tour/GetCurrentBookings`, 
            { 
                params: { filter: filter },
            });  
            commit('setCurrentBookings', tours);          
        }
        catch (error) {
            console.log(error);
        }        
    },
    async cancelTour({ commit }, id) {
        try {
            return await this.$axios.$post<boolean>(config.bustourApiUrl + `/Tour/Cancel`, { id: id });
        } catch (error) {
            throw (error as any).response.data
        }
    },    
    async deleteTour(_, tourId) {
        try {
            return await this.$axios.$post<BaseResponse>(`${config.bustourApiUrl}/Tour/DeleteTour`, { id: tourId });
        } catch (error) {
            return {
                isSuccess: false,
                message: "Something went wrong...",
            } as BaseResponse;
        }
    },
    async updateServiceTour(_, tour: Tour) {
        try {
            let tourCopy = {...tour};

            tourCopy.departure = new Date((tour?.departure?.toString() ?? "") + "+0000");

            return await this.$axios.$put<BaseResponse>(`${config.bustourApiUrl}/Tour/UpdateServiceTour`, tourCopy);
        } catch (error) {
            return { 
                isSuccess: false,
                message: "Something went wrong...",
            } as BaseResponse;
        }
    },
};

export const mutations: MutationTree<TourState> = {
    setToursCreation(state: TourState, data: TourCreation) {
        state.toursCreation = {...state.toursCreation, ...data};   
    },
    addTourCreation(state: TourState, data: TourCreation) {
        state.toursCreation.tours.push(Object.assign(new TourCreation(), data))
    },
    setCities(state: TourState, data: City[]) {
        Vue.set(state, 'cities', data);
    },   
    setRoutes(state: TourState, data: Route[]) {
        state.routes = data;
    },      
    setTourMenuBeverage(state: TourState, data: TourMenuBeverage): void {
        state.tourMenuBeverage = data;
    },
    setTourById(state: TourState, data: Tour): void {
        state.tour = data;
    },
    setTourFB(state: TourState, data: TourFB): void {
        state.tourFB = data;
	},
    setTourInformation(state: TourState, payload: ITourInformation): void {
        state.tourInfo = payload;
        state.tourInfo.tourSummary.city = state.cities.find(c => c.id == payload.tourSummary.cityId)?.name ?? {} as Dictionary<string>;
    },
    setCurrentBookings(state: TourState, data: CurrentBooking[]): void {
        state.currentBookings = data;
	},
    setBuses(state: TourState, data: Bus[]): void {
        data.forEach(b => b.tables.forEach(t => t.seats.forEach(s => s.tableId = t.id))) 
        state.buses = data;
	},    
};

export default {
    state,
    actions,
    mutations,
    getters
}