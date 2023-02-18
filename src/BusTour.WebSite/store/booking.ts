import config from "@/config";
import Vue from "vue";

import { 
	RootState, Language, RouteInfo, Table, Seat, MenuInfo, 
	OrderState, OrderStep, BusSchemeItem, BaseResponse, OrderResponse,
	ResponseSelection, OrderMenu, OrderSeat, OrderTable,
	OrderBeverage, OrderSurprise, SeatingType, OrdersConflictsResponse,
	TestInfo, TableCategoryEnum, Order, Route, Tour, Client, Bus, SeatType,
	MenuTypeEnum, DebugInfo, SelectionInfo, BusObject, BusObjectTypes, 
	ResponseTable, OrderType, OrderInfoSeat, OrderExtras, OrderPrivateHire, Payment,
	CalculationCostTourResponse, UpgradeOrderRequest, ResponseUpgradeTable, SchemeTableModel, SchemeSeatModel, OrderInfoTable, OrderInfo } from '~/types/booking/index'
import { OrderStateMovingCommand, Time } from '@/types/common'	

import { ActionTree, GetterTree, MutationTree } from 'vuex'
import { TourPrivateHire, TourType } from "~/types/tour";
import { GiftCertificate } from "~/types/giftCertificate";
import { filter } from "vue/types/umd";
import { Promocode } from "~/types/promocodes/index"

	const defaultOrder = {
		id: 0,
		hash: '',
		orderState: OrderState.Draft,
		step: OrderStep.Form,
		type: OrderType.Regular,
		guestCount: 1,
		date: new Date(),
		guestsWithDisabilities: false,
		disabledGuestCount: 0,
		seatingType: SeatingType.BySeats,
		tourId: 0,
		routeId: null,
		tour: null,
		totalPrice: 0,
		client: {
			id: 0,
			phoneNumber: '',
			email: '',
			isSigned: false
		},
		name: null,
		phone: null,
		specialRequests: null,
		comments: null,
		seats: [],
		tables: [],
		menus: [],
		beverages: [],
		surprises: [],
		privateHire: new OrderPrivateHire(),
		comment: null,
		payment: <Payment>{},
		isGroup: false,
		conflictsResponse: null
	} as Order;

	const defaultPayment = {
		cardholdersName: '',
		cardNumber: '',
		cardVerificationCode: '',
		name: '',
		email: '',
		repeatemail: '',
		mounth: 1,
		year: 2021,
		phone: {
			code: '',
			number: '',
		}
	} as Payment;

	
	export const state = (): RootState => ({
		languages: [],
		selectedLanguageCode: 'en',
		selectedRouteId: 1,//London
		menuInfo: {
			menus: [],
			beverages: [],
			allergies: [],
			surprises: []
		},
		routeInfo: {
			route: {
				id: 0,
				name: {},
				description: {},
				duration: '',
				departureAddress: {},
				departureHowToGet: {},
				cityName: {},
				titleImgPath: '',
				mapImgPath: '',
				imgPaths: [],
				cityId: 0
			},
			tours: []
		},
		selectionModel: {
			isSuccess: false,
			tables: [],
			selection: {
				tourId: 0,
				guestCount: 0,
				seatingType: SeatingType.SeparateTable,
				selectedObjects: [],
				clickedObject: null,
				manualSelectionMode: false,
				orderType: OrderType.Regular
			},
			orderInfo: {
				guests: [],
				tables: []
			}
		},
		orderCertificate: {
			id: null,
			amountVariant: {
				id: 0,
				amount: 0,
			},
			dateStart: null,
			dateEnd: null,
			certificateSurprises: [],
			comment: '',
			number: null,
			status: null,
			payment: null
		},
		orderPromocode: {
			id: 0,
			seriesNumber: '',
			promoCodeType:'',
			dateStart: undefined,
			dateEnd: undefined,
			numberOfPromocodes: undefined,
			amountOfDiscount: undefined,
			typeOfDiscount: '',
			cityId: undefined,
		},
		hoveredSchemeItem: {
			seat: undefined,
			table: undefined
		},
		order: defaultOrder,
		orderSeats: [],
		orderExtras: {
			menus: [],
			beverages: [],
		},
		actionResult: {
			isSuccess: true
		},
		testInfo: {
			selectionResult: {
				isAutoSelect: false,
				path: []
			},
			selection: {} as SelectionInfo,
			tables: {}
		},
		debugInfo: {
			table: {
				rows: []
			},
			legend: {
				rows: []
			}
		},
		payment: defaultPayment,
		tour: {
			id: 0,
			departure: '',
			arrival: '',
			tables: [],
			privateHire: null,
			routeId: null,
			busId: 0,
			isAvailableForBooking: true,
			occupiedSeatsCount: 0,
		},
		buses: [],
		calculationCostTour: {
			tourPrice: 0,
			vat: 0,
			totalPrice: 0
		},
		isUpgradeMode: false,
		schemeTables: [],

	})
	
	interface Actions<S, R> extends ActionTree<S, R> { }
	
	export const actions: Actions<RootState, RootState> = {
		async nuxtServerInit({dispatch }, context) {
			await dispatch('getLanguages');
			await dispatch('getMenuInfo');
			await dispatch('getRouteInfo');
		},
		async getLanguages({ commit }) {
			try {
				const response = await this.$axios.$get<Language[]>(config.bustourApiUrl + '/Reference/GetLanguages');
	
				commit('setLanguages', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getMenuInfo({ commit }) {
			try {
				const response = await this.$axios.$get<MenuInfo>(config.bustourApiUrl + '/Tour/GetMenuInfo');
	
				commit('setMenuInfo', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getRouteInfo({ state, commit, dispatch }, payload: any) {
			try {

				const routeId = payload?.routeId ?? state.selectedRouteId;

				const response = await this.$axios.$get<RouteInfo>(config.bustourApiUrl + `/Tour/GetRouteInfo/${routeId}`);

				commit('setRouteInfo', response);

				if (!payload?.ignoreGuestCountReset) {
					commit("setGuestCount", 1);
				}
	
				await dispatch("getSelectionModel", null);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getBuses({ state, commit, dispatch }) {
			try {
				if (!state.buses?.length) {
					const response = await this.$axios.$get<Bus[]>(config.bustourApiUrl + `/Tour/GetBuses`)
					commit('setBuses', response)
					return response
				} else {
					return state.buses;
				}
			}
			catch (error) {
				console.log(error);
			}
		},		
		async getOrder({ state, commit, dispatch }, id: number) {
			try {
				const response = await this.$axios.$get<Order>(config.bustourApiUrl + `/Order/${id}`);
				
				commit('setOrder', response);

				return response
			}
			catch (error) {
				console.log(error);
			}
		},
		async updateOrderSeat({ state, commit, dispatch }, seat: OrderSeat) {
			try {
				await this.$axios.$put<OrderSeat>(config.bustourApiUrl + '/Order/UpdateOrderSeat', seat);
	
				commit('updateOrderSeat', seat);
			}
			catch (error) {
				console.log(error);
			}
		},
		async updateOrderMenu({ state, commit, dispatch }, menu: OrderMenu) {
			try {
				await this.$axios.$put<OrderMenu>(config.bustourApiUrl + '/Order/UpdateOrderMenu', menu);
				
				commit('updateOrderExtraMenu', menu);
			}
			catch (error) {
				console.log(error);
			}
		},
		async updateOrderBeverage({ state, commit, dispatch }, beverage: OrderBeverage) {
			try {
				await this.$axios.$put<OrderSeat>(config.bustourApiUrl + '/Order/UpdateOrderBeverage', beverage);
	
				commit('updateOrderExtraBeverage', beverage);
			}
			catch (error) {
				console.log(error);
			}
		},
		async allGuestHasCome({ state, commit, dispatch }, orderId: number) {
			try {
				await this.$axios.$put<boolean>(config.bustourApiUrl + 	`/Order/AllGuestHasCome/${orderId}`);
	
				commit('allGuestHasCome', orderId);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getOrderExtras({ state, commit, dispatch }, id: number) {
			try {
				const response = await this.$axios.$get<OrderExtras>(config.bustourApiUrl + `/Order/Extras/${id}`);
				
				commit('setOrderExtras', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getTour({ state, commit }, id: number) {
			try {
				const response = await this.$axios.get<Tour>(config.bustourApiUrl + `/Tour/${id}`);
				commit('setCurrentTour', response.data);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getSelectionModel({ state, commit }, payload) {
			try {
				const parameters = <SelectionInfo>{
					tourId: state.order.tourId,
					guestCount: state.order.guestCount,
					seatingType: state.order.seatingType,
					clickedObject: payload ? payload.clickedObject : null,
					selectedObjects: state.selectionModel.selection.selectedObjects,
					manualSelectionMode: payload ? payload.manualSelectionMode : false,
					orderType: state.order.type,
					disabledGuestCount: state.order.disabledGuestCount
				};
	
				const response = await this.$axios.$post<ResponseSelection>(config.bustourApiUrl + '/Selection/get-bus-model', parameters, { withCredentials: true });
				commit("setSelectionModel", response);

				if(payload?.clickedObject?.id === 0)
				{
					commit("setSelectedSeats");
					commit("seterOrderSeatFullNameAndSort");
				}
				
			}
			catch (error) {
				console.log(error);
			}
			
			
		},
		async getEditableBusModel({ commit, state }, { clickedObject = null } = {}) {
			const parameters = <SelectionInfo>{
				tourId: state.order.tourId,
				guestCount: state.order.guestCount,
				seatingType: state.order.seatingType,
				clickedObject: clickedObject,
				selectedObjects: state.selectionModel.selection.selectedObjects,
				manualSelectionMode: false,
				orderType: state.order.type,
				orderId: state.order.id,
				toString() {
					return JSON.stringify(this);
				},
			};

			const response = await this.$axios.$get<ResponseSelection>(`${config.bustourApiUrl}/Selection/GetSelectableBusModelForOrder?info=${parameters}`);
			commit("setSelectionModel", response);
		},
		async changeSeatOrUpgrade({ commit, dispatch }) {
			await dispatch("getSelectionModel", {manualSelectionMode: true});
		},
		async selectSeat({ commit, dispatch }, payload: number) {
			commit("setHoveredObject", <BusSchemeItem>{
				table: undefined,
				seat: undefined,
			});
			
			commit("selectSeat", payload);
	
			const clickedObject =  <BusObject> {
				type: BusObjectTypes.Seat,
				id: payload
			};
			await dispatch("getSelectionModel", {clickedObject: clickedObject});
		},
		async changeSeatInEditingMode({ commit, dispatch }, payload: number) {
			commit("setHoveredObject", <BusSchemeItem>{
				table: undefined,
				seat: undefined,
			});
			
			commit("selectSeat", payload);
	
			const clickedObject =  <BusObject> {
				type: BusObjectTypes.Seat,
				id: payload
			};
			await dispatch("getEditableBusModel", {clickedObject: clickedObject});
		},
		async selectTable({ commit, dispatch }, payload: number) {
			commit("setHoveredObject", <BusSchemeItem>{
				table: undefined,
				seat: undefined,
			});
	
			commit("selectTable", payload);
	
			const clickedObject =  <BusObject> {
				type: BusObjectTypes.Table,
				id: payload
			};
	
			await dispatch("getSelectionModel", {clickedObject: clickedObject});
		},
		async setGuestCount({ commit, dispatch }, payload: number) {
			commit("setGuestCount", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async setGuestsWithDisabilities({ commit, dispatch }, payload: boolean) {
			commit("setGuestsWithDisabilities", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async setDisabledGuestCount({ commit, dispatch }, payload: number) {
			commit("setDisabledGuestCount", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async setSeatingType({ commit, dispatch }, payload: SeatingType) {
			commit("setSeatingType", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async setOrderTime({ commit, dispatch }, payload: string) {
			commit("setOrderTime", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async setOrderDate({ commit, dispatch }, payload: string) {
			commit("setOrderDate", payload);
	
			await dispatch("getSelectionModel", null);
		},
		async createOrUpdateOrderDraft({ state, commit }) {
			try {
				commit("resetEmptyMenu");

				var response = await this.$axios.$post<OrderResponse>(config.bustourApiUrl + '/Order/create-update-draft', state.order);
	
				commit('setOrder', { id: response.orderId, hash: response.hash });

				return true;
			}
			catch (error) {
				console.log(error);

				return false;
			}
		},
		async sendOrderPaymentSuccess({ state, commit }) {
			try {
				var response = await this.$axios.$post<Payment>(config.bustourApiUrl + '/Payment/OrderPaymentSuccess', {
					orderId: state.order.id,
					client: {
						fullName: state.payment.name,
						email: state.payment.email,
						phoneNumber: state.payment.phone.code + ' ' +  state.payment.phone.number
					}

				});
				return response;
			}
			catch (error) {
				console.log(error);
			}
		},		
		async sendOrderPaymentFail({ state, commit }, error: string) {
			try {
				var response = await this.$axios.$post<Payment>(config.bustourApiUrl + '/Payment/OrderPaymentFail', {
					orderId: state.order.id,
					error: error
				});
				return response;
			}
			catch (error) {
				console.log(error);
			}
		},			
		async setForPayment({ state, commit }) {
			try {
				var response = await this.$axios.$post<OrderResponse>(config.bustourApiUrl + '/Order/set-for-payment', state.order);
				return response;
			}
			catch (error) {
				console.log(error);
			}
		},	
		async createPrivateHireOrder({ state, commit }) {
			try {
				var response = await this.$axios.$post<Order>(config.bustourApiUrl + '/Order/CreatePrivateHireOrder', state.order);
				commit('setOrder', { id: response.id, hash: response.hash });
				
				return true;
			}
			catch (error) {
				console.log(error);

				return false;
			}
		},	
		async getBusModel({ commit }, payload) {
			try {
				const response = await this.$axios.$post<TestInfo>(config.bustourApiUrl + `/TestSelection/get-bus-model`, payload, { withCredentials: true });
	
				commit('setTestInfo', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async changeSelection({ commit }, payload) {
			try {
				const response = await this.$axios.$post<TestInfo>(config.bustourApiUrl + `/TestSelection/selection-switch`, payload, { withCredentials: true });
	
				commit('setTestInfo', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async changeLock({ commit }, payload) {
			try {
				const response = await this.$axios.$post<TestInfo>(config.bustourApiUrl + `/TestSelection/lock-switch`, payload, { withCredentials: true });
	
				commit('setTestInfo', response);
			}
			catch (error) {
				console.log(error);
			}
		},
		async loadDebugInfo({ commit }, payload) {
			try {
				const response = await this.$axios.$post<DebugInfo>(config.bustourApiUrl + `/TestSelection/get-rules-debug-info`, payload, { withCredentials: true });
	
				commit('setDebugInfo', response);
			}
			catch (error) {
				console.log(error);
			}
		},	
		async getOrderConflicts({ state, commit }) {
			try {
				if (state.order?.id) {
					return await this.$axios.$get<OrdersConflictsResponse>(config.bustourApiUrl + `/Order/get-conflicts/${state.order.id}`)
				} else {
					return await this.$axios.$post<OrdersConflictsResponse>(config.bustourApiUrl + '/Order/get-conflicts', state.order)
				}
			}
			catch (error) {
				console.log(error);
			}
		},				
		async getOrderByHash({ state }, hash) {
			try {
				console.log(hash);
				
				return await this.$axios.$get<Order>(config.bustourApiUrl + '/Order/ByHash/' + hash)
			}
			catch (error) {
				console.log(error);
			}
		},	
		async cancelOrder({ commit }, id) {
			try {
				return await this.$axios.$post<boolean>(config.bustourApiUrl + `/Order/Cancel`, { id: id });
			} catch (error) {
				throw (error as any).response.data
			}
		},
		async addNotification({ state}, {email, lang} ) {
			try {
				return await this.$axios.$post<boolean>(config.bustourApiUrl + '/Order/AddNotification',{
					orderId: state.order.id,
					email: email,
					lang: lang
				})
			}
			catch (error) {
				console.log(error);
			}
		},
		async getCalculationCostTour({ state, commit }) {
			try {
				var response = await this.$axios.$post<CalculationCostTourResponse>(config.bustourApiUrl + '/Order/GetCalculationCostTour', state.order);

				commit('setCalculationCostTour', response)

				return true;
			}
			catch (error) {
				console.log(error);

				return false;
			}
		},
		setPrivateHireOrderState({ state, commit }, order: Order) {
			commit('setRegularTourEditingOrderState', order)
			commit('setPrivateHireOrderEditingOrderState', order)
		},
		async upgradeOrder({ state, commit, rootState }, data: BusObject) {
			try {
				const request = {
					orderId: state.order.id,
					clickedObject: {
						id: data.id,
						type: data.type
					} as BusObject
				} as UpgradeOrderRequest;

				const response = await this.$axios.post<ResponseUpgradeTable>(config.bustourApiUrl + `/Order/upgrade`, request) as any;
				
				commit('upgradeOrder', { tables: response.data.tables, type: data.type, busTables: ((rootState.tour as any).buses[0].tables as Table[]) } );
				commit('setSchemeTables', response.data.tables);
			}
			catch (error) {
				console.log(error);
			}
		},
		async setSchemeTables({ state, commit }, id: Number) {
			try {
				const response = await this.$axios.get<ResponseUpgradeTable>(config.bustourApiUrl + `/Order/get-bus-model/${id}`) as any;
				commit('setSchemeTables', response.data.tables);
			}
			catch (error) {
				console.log(error);
			}
		},
		async getCanBeUpgraded({}, id: Number) {
			try {
				return await this.$axios.$get<Order>(config.bustourApiUrl + `/Order/can-be-upgraded/${id}`)
			}
			catch (error) {
				console.log(error);
			}
		},	
	}
	
	export const mutations: MutationTree<RootState> = {
		upgradeOrder(state: RootState, data) {
			const tables = data.tables as ResponseUpgradeTable[]
			const type = data.type as BusObjectTypes;

			if(state.order.seatingType == SeatingType.BySeats && type == BusObjectTypes.Table) {
				state.order.seatingType = SeatingType.SeparateTable;
			}

			//if(type == BusObjectTypes.Seat) {
				let newOrderSeats = [] as OrderSeat[];
				tables.filter(t => t.seats.some(s => s.isSelected)).forEach(t => {
					t.seats.forEach(s => {
						if(s.isSelected) {
							newOrderSeats.push(<OrderSeat>{ seatId: s.id });
						}
					})
				})
				Vue.set(state.order, 'seats', newOrderSeats);
			//}
			//if(type == BusObjectTypes.Table) {
				let newOrderTables = [] as OrderTable[];
				tables.filter(t => t.isSelected).forEach(x => {
					newOrderTables.push(<OrderTable>{ tableId: x.id });
				});
				state.order.tables = newOrderTables;

				console.log(state.order.tables);
			//}
		},
		createSchemeTables(state: RootState) {
			const tour = state.routeInfo.tours.find(x => x.id == state.order.tourId);
			if(tour === undefined) {
				return;
			}
			
			state.schemeTables = tour.tables.map((x) => {
				return  {
					id: x.id,
					number: x.number,
					x: x.x,
					y: x.y,
					xSize: x.xSize,
					ySize: x.ySize,
					isLeft: x.isLeft,
					isRight: x.isRight,
					floor: x.floor,
					price: x.price,
					category: x.category,
					isAvailable: false,
					isAvailableForUpgrade: false,
					initialPrice: 0,
					isSelected: false,
					seats: x.seats.map((s) => {
						return {
							id: s.id,
							name: s.name,
							x: s.x,
							y: s.y,
							isForward: s.isForward,
							isBackward: s.isBackward,
							price: s.price,
							initialPrice: 0,
							isSelected: false,
							isAvailable: false,
							rotate: s.rotate,
  							type: s.type,
							scaleX: s.scaleX,
							scaleY: s.scaleY
						}
					}) as SchemeSeatModel[],
				} as SchemeTableModel;
			})

			// установка isSelected для seats
			state.order.seats.forEach(s => {
				let seat = state.schemeTables.find(t => t.seats.find(x => x.id == s.seatId))?.seats.find(x => x.id == s.seatId);
				if(seat !== undefined) {
					seat.isSelected = true;
				}
			})

			// установка isSelected для tables
			state.order.tables.forEach(t => {
				let table = state.schemeTables.find(x => x.id == t.tableId);
				if(table !== undefined) {
					table.isSelected = true;
				}
			})

		},
		setSchemeTables(state: RootState, tables: ResponseUpgradeTable[]) {
			state.schemeTables.forEach((t) => {
				const table = tables.find(x => x.id == t.id);
				if(table != undefined) {
					t.price = table.price;
					t.isAvailable = table.isAvailable;
					t.initialPrice = table.initialPrice;
					t.isSelected = table.isSelected;
					t.isAvailableForUpgrade = table.isAvailableForUpgrade;
					t.seats.forEach((s) => {
						const seat = table.seats.find(x => x.id == s.id);
						if(seat != undefined) {
							s.price = seat.price;
							s.initialPrice = seat.initialPrice;
							s.isSelected = seat.isSelected;
							s.isAvailable = seat.isAvailable;
						}					
					})
				}
			})
		},
		setPayment(state: RootState, data: Payment) {
			state.payment = {...state.payment, ...data}
		},		
		resetPayment(state: RootState, data: Payment) {
			state.payment = {...state.payment, ...defaultPayment}
		},				
		setOrderClient(state: RootState, data: Client) {
			state.order.client = {...state.order.client, ...data}
		},		
		setOrderPrivateHire(state: RootState, data: OrderPrivateHire) {
			console.log(data)
			state.order.privateHire = {...state.order.privateHire, ...data}
		},
		setOrderSeatsForOrderInfo(state, commit) {
			let tour = state.order.tour as Tour;

			state.orderSeats = state.order.seats;
			state.order.seats = [];

			state.order.guestCount = state.orderSeats.length;

			//add tables to order
			state.orderSeats.forEach((seat)=> {
				let tableId = tour.tables.find(x => x.seats.find(x => x.id === seat.seatId))?.id;
				if(state.order.tables.find(x => x.tableId === tableId) === undefined) {
					state.order.tables.push({tableId: tableId as number})
				}
			});
			
			state.order.tables.forEach((item) => {
				tour.tables.find(x => x.id === item.tableId)?.seats.forEach((seat)=> {
					let orderSeat = state.orderSeats.find(x => x.seatId === seat.id);
					state.order.seats.push({
						id: orderSeat?.id || 0,
						orderId: orderSeat?.orderId || 0,
						seatId: seat.id,
						menuId: orderSeat?.menuId,
						beverageId: orderSeat?.beverageId || undefined,
						isAllergy: 	orderSeat?.allergyId != null || 
									orderSeat?.allergyId != undefined || 
									orderSeat?.otherAllergy != null  || 
									orderSeat?.otherAllergy != undefined,
						allergyId: orderSeat?.allergyId,
						seatFullName: "",
						otherAllergy: orderSeat?.otherAllergy,  
						guestHasCome: orderSeat?.guestHasCome || false,
						hasMenuIssued: orderSeat?.hasMenuIssued || false,
						hasBeverageIssued: orderSeat?.hasBeverageIssued || false,
						rotate: orderSeat?.rotate || 0,
  						type: orderSeat?.type || 0,
						scaleX: orderSeat?.scaleX || 0,
						scaleY: orderSeat?.scaleY || 0,
					})
				});
			});
			
			state.routeInfo.tours = [tour];

			state.selectionModel.selection.selectedObjects = state.order.seats.map((item)=> {
				return {
					type: 2,
					id: item.seatId
				}
			});
		},
		updateOrderSeat(state: RootState, seat: OrderSeat): void {
			let orderSeat = state.order.seats.find(x => x.id === seat.id);
			if(orderSeat === undefined) {
				console.log("Seat not found");
				return;
			}

			orderSeat.guestHasCome = seat.guestHasCome;
			orderSeat.hasBeverageIssued = seat.hasBeverageIssued;
			orderSeat.hasMenuIssued = seat.hasMenuIssued;
		},
		updateOrderExtraMenu(state: RootState, menu: OrderMenu): void {
			let extraMenu = state.orderExtras.menus.find(x => x.id === menu.id);
			if(extraMenu === undefined) {
				console.log("Menu not found");
				return;
			}

			extraMenu.menuId = menu.menuId,
			extraMenu.amount = menu.amount,
			extraMenu.issued = menu.issued
		},
		updateOrderExtraBeverage(state: RootState, beverage: OrderBeverage): void {
			let extraBeverage = state.orderExtras.beverages.find(x => x.id === beverage.id);
			if(extraBeverage === undefined) {
				console.log("Menu not found");
				return;
			}
			
			extraBeverage.beverageId = beverage.beverageId,
			extraBeverage.amount = beverage.amount,
			extraBeverage.issued = beverage.issued
		},
		setSelectedSeats(state: RootState): void {
			state.selectionModel.orderInfo.tables.forEach((item) => {

				item.seats.forEach((seat) => {
					if(state.orderSeats.find(x => x.seatId == seat.id) === undefined) {
						seat.isSelected = false;
					}
				})
			})
			
		},
		allGuestHasCome(state: RootState): void {
			state.order.seats.forEach(x => x.guestHasCome = true);
		},
		seterOrderSeatFullNameAndSort(state: RootState) : void {
			state.orderSeats.forEach(item =>{
				item.seatFullName = state.selectionModel.orderInfo.guests.find(x => x.seatId === item.seatId)?.seatFullName ?? '';
			})
			
			state.orderSeats.sort();
		},
		setLanguages(state: RootState, data: Language[]): void {
			state.languages = data;
		},
		setMenuInfo(state: RootState, data: MenuInfo): void {
			state.menuInfo = data;
		},
		setRouteInfo(state: RootState, data: RouteInfo): void {
			state.routeInfo = data;
	
			if (state.routeInfo && state.routeInfo.tours && state.routeInfo.tours.length > 0) {
				// state.order.date = state.routeInfo.tours[0].departure.split("T")[0];
				// state.order.time = state.routeInfo.tours[0].departure.split("T")[1];
	
				const tours = state.routeInfo.tours
					.filter((item) => item.departure === state.order.date + "T" + state.order.time);
	
				if (tours && tours.length === 1) {
					state.order.tourId = tours[0].id;
				}
			}
		},
		setBuses(state: RootState, data: Bus[]): void {
			data.forEach(b => b.tables.forEach(t => t.seats.forEach(s => s.tableId = t.id)));	
			state.buses = [...state.buses, ...data]
		},
		setOrder(state: RootState, data: Order): void {
			state.order = {...state.order, ...data};
		},
		
		resetOrder(state: RootState, data: Order | null = null) {
			const constants = { menus: state.order.menus, beverages: state.order.beverages }
			state.order = {...state.order, ...{...{...defaultOrder,...constants}, ...(data ? data : {})}}
		},
		resetOrderSeats(state: RootState) {
			state.order.seats = [];
		},		
		setOrderExtras(state: RootState, data: OrderExtras): void {	
			state.orderExtras = data;
		},
		setActionResult(state: RootState, data: BaseResponse): void {
			state.actionResult = data;
		},
		setOrderSeat(state: RootState, data: OrderSeat): void {

			const existingItems = state.order.seats.filter((item) => item.seatId === data.seatId);
			if (existingItems.length === 0) {
				state.order.seats.push(data);
			}
		},
		setOrderMenu(state: RootState, data: OrderMenu): void {
			const existingItems = state.order.menus.filter((item) => item.menuId === data.menuId);
			if (existingItems.length > 0) {
				existingItems[0].amount = data.amount;
			}
			else {
				state.order.menus.push(data);
			}
		},
		setOrderBeverage(state: RootState, data: OrderBeverage): void {
			const existingItems = state.order.beverages.filter((item) => item.beverageId === data.beverageId);
			if (existingItems.length > 0) {
				existingItems[0].amount = data.amount;
			}
			else {
				state.order.beverages.push(data);
			}
		},
		setOrderSurprise(state: RootState, data: OrderSurprise): void {
			const existingItems = state.order.surprises.filter((item) => item.surpriseId === data.surpriseId);
			if (existingItems.length > 0) {
				existingItems[0].amount = data.amount;
			}
			else {
				state.order.surprises.push(data);
			}
		},
		setGuestCount(state: RootState, data: number): void {
			if (data > 0) {
				state.order.guestCount = data;
	
				state.menuInfo.menus
					//.filter((item) => item.menuType.id !== MenuTypeEnum.Main)
					.forEach((menu) => {
					let orderMenus = state.order.menus.filter((om) => om.menuId === menu.id);
					if (orderMenus.length === 0) {
						state.order.menus.push(<OrderMenu>{ menuId: menu.id, amount: 0 });
					}
				});
				
				state.menuInfo.beverages.forEach((beverage) => {
					let orderBeverages = state.order.beverages.filter((ob) => ob.beverageId === beverage.id);
					if (orderBeverages.length === 0) {
						state.order.beverages.push(<OrderBeverage>{ beverageId: beverage.id, amount: 0 });
					}
				});
			}
		},
		setGuestsWithDisabilities(state: RootState, data: boolean): void {
			state.order.guestsWithDisabilities = data;
		},
		setDisabledGuestCount(state: RootState, data: number): void {
			state.order.disabledGuestCount = data;
		},
		setSeatingType(state: RootState, data: SeatingType): void {
			state.order.seatingType = data;
	
			state.order.seats = [];
			state.order.tables = [];
		},
		setOrderTime(state: RootState, data: string): void {
			// state.order.time = data;
	
			const tours = state.routeInfo.tours
				.filter((item) => item.departure === state.order.date + "T" + state.order.time);
	
			if (tours && tours.length === 1) {
				state.order.tourId = tours[0].id;
			}
		},
		setOrderDate(state: RootState, data: string): void{
			// state.order.date = data;
	
			const tours = state.routeInfo.tours
				.filter((item) => item.departure === state.order.date + "T" + state.order.time);
	
			if (tours && tours.length === 1) {
				state.order.tourId = tours[0].id;
			}
		},
		setGiftCertificate(state: RootState, data: string): void {
			state.order.certificateId = data;
		},
		setTestInfo(state: RootState, data: TestInfo): void {
			state.testInfo = data;
		},
		setDebugInfo(state: RootState, data: DebugInfo): void {
			state.debugInfo = data;
		},
		selectSeat(state: RootState, data: number) {			
			if (state.order.seats.some((item) => item.seatId === data)) {
				state.order.seats = state.order.seats.filter((item) => item.seatId !== data);
			}
			else {
				state.order.seats.push(<OrderSeat>{ seatId: data });
			}
		},
		selectTable(state: RootState, data: number) {
			if (state.order.tables.some((item) => item.tableId === data)) {
				state.order.tables = state.order.tables.filter((item) => item.tableId !== data);
			}
			else {
				state.order.tables.push(<OrderTable>{ tableId: data });
			}
		},
		goToPrevOrderStep(state: RootState) {
			state.order.step--;
		},
		goToNextOrderStep(state: RootState) {
			state.order.step++;
		},
		goToOrderStep(state: RootState, step: OrderStep) {
			state.order.step = step;
		},
		setSelectionModel(state: RootState, data: ResponseSelection) {
			
			if (data.isSuccess) {
				state.selectionModel = data;
	
				// state.order.seatingType = data.selection.seatingType;
	
				let newSelectedTables = [] as OrderTable[];
				let newSelectedSeats = [] as OrderSeat[];
	
				data.selection.selectedObjects.forEach((item)=>{
					if(item.type == BusObjectTypes.Table)
						newSelectedTables.push(<OrderTable>{tableId: item.id})
				});
				data.selection.selectedObjects.forEach((item)=>{
					if(item.type == BusObjectTypes.Seat)
					newSelectedSeats.push(<OrderSeat>{seatId: item.id})
				});

				let guests = data.orderInfo.guests;
				let newMainMenus = state.order.seats.filter(item => (guests.some((p) => item.seatId === p.seatId)));
	
				state.order.tables = newSelectedTables;
				state.order.seats = newSelectedSeats;
	
				guests.forEach((item)=>{
					if (!(newMainMenus.some((p) => item.seatId === p.seatId)))
						newMainMenus.push(<OrderSeat>{seatId: item.seatId, seatFullName: item.seatFullName})
					else
						newMainMenus.filter(x=>x.seatId===item.seatId)[0].seatFullName = item.seatFullName
				});

				state.order.seats = newMainMenus;
			}
		},
		clearSelectedObjects(state: RootState) {
			state.selectionModel.selection.selectedObjects = [];
		},
		removeSelectedObjects(state: RootState, data: number[]) {
			state.selectionModel.selection.selectedObjects = state.selectionModel.selection.selectedObjects.filter(x => !data.includes(x.id));
		},		
		setHoveredObject(state: RootState, data: BusSchemeItem) {
			state.hoveredSchemeItem = data;
		},
		setMainMenuForGuest(state: RootState, data: OrderSeat) {
			const orderMenu = state.order.seats.filter((item) => item.seatId === data.seatId)[0];
	        Vue.set(orderMenu, 'menuId', data.menuId);
	    },
	    setMainBeverageForGuest(state: RootState, data: OrderSeat) {
	    	const orderBeverage = state.order.seats.filter((item) => item.seatId === data.seatId)[0];
	    	Vue.set(orderBeverage, 'beverageId', data.beverageId);
	    },
		setAllergyForGuest(state: RootState, data: OrderSeat) {
	    	const orderAllergy = state.order.seats.filter((item) => item.seatId === data.seatId)[0];
	    	Vue.set(orderAllergy, 'isAllergy', data.isAllergy);
			if(!data.isAllergy){
				Vue.set(orderAllergy, 'allergyId', undefined);
				Vue.set(orderAllergy, 'otherAllergy', "");
			}
	    }, 
	    setAllergyNameForGuest(state: RootState, data: OrderSeat) {
	    	const orderAllergyName = state.order.seats.filter((item) => item.seatId === data.seatId)[0];
	    	Vue.set(orderAllergyName, 'allergyId', data.allergyId);
	    },    
	    setOtherAllergyForGuest(state: RootState, data: OrderSeat) {
	    	const orderOtherAllergy = state.order.seats.filter((item) => item.seatId === data.seatId)[0];
	    	Vue.set(orderOtherAllergy, 'otherAllergy', data.otherAllergy);
	    },
		setOrderBeverageAmount(state: RootState, data: OrderBeverage) {
			const orderBeverage = state.order.beverages.find(b => b.beverageId === data.beverageId);

			if (orderBeverage) {
				orderBeverage.amount = data.amount;
			} else {
				const tourBeverage = (<OrderBeverage[]>(<any>state.order.tour).tourBeverages).find(b => b.beverageId === data.beverageId);

				if (!tourBeverage) {
					throw new Error(`Beverage with id = ${data.beverageId} not found`);
				} 

				let newOrderBeverage = {} as OrderBeverage;

				newOrderBeverage.amount = data.amount;
				newOrderBeverage.beverageId = data.beverageId;

				state.order.beverages.push(newOrderBeverage);
			}
		},
		setOrderMenuAmount(state: RootState, data: OrderMenu) {
			let orderMenu = state.order.menus.find(x => x.menuId === data.menuId);

			if (!orderMenu) {
				orderMenu = {
					menuId: data.menuId,
					amount: data.amount,
					id: 0,
					orderId: state.order.id,
					issued: true
				}
				state.order.menus.push(orderMenu)
			}
			else {
				orderMenu.amount = data.amount;
			}
		},
		setCardholdersName(state: RootState, data: string) {
			state.payment.cardholdersName = data;
		},
		setCardNumber(state: RootState, data: string) {
			state.payment.cardNumber = data;
		},
		setCardVerificationCode(state: RootState, data: string){
			state.payment.cardVerificationCode = data;
		},
		setName(state: RootState, data: string){
			state.payment.name = data;
		},
		setEmail(state: RootState, data: string){
			state.payment.email = data;
		},
		setRepeatemail(state: RootState, data: string){
			state.payment.repeatemail = data;
		},
		setMounth(state: RootState, data: number){
			state.payment.mounth = data;
		},
		setYear(state: RootState, data: number){
			state.payment.year = data;
		},
		setOrderId(state: RootState, data: number): void {
			state.order.id = data;
		},
		setServiceTour(state: RootState, data: any) {
			state.order.tour = {...state.order.tour, ...data};
		},
		setTour(state: RootState) {
			const date = state.order.date;
			const time = Time.fromPlain(state.order.time as Time);
			const tour = state.routeInfo.tours.find(tour => Date.parse(tour.departure) === Date.parse(`${date?.toDateString()} ${time?.toString()}`));

			if (tour?.id === state.order.tourId) {
				return;
			}

			state.order.tourId = tour?.id ?? 0;
			state.order.menus?.forEach(item => {
				item.amount=0
			});
			state.order.beverages?.forEach(item => {
				item.amount=0
			});

		},
		setCurrentTour(state: RootState, data: Tour) {
			state.tour = data;
		},
		setOrderCertificate(state: RootState, data: GiftCertificate) {
			state.order.certificateId = data.id?.toString();
			state.orderCertificate = data;
		},
		resetOrderCertificate(state: RootState) {
			state.order.certificateId = undefined;
			state.orderCertificate = {
				id: null,
				amountVariant: {
					id: 0,
					amount: 0,
				},
				dateStart: null,
				dateEnd: null,
				certificateSurprises: [],
				comment: '',
				number: null,
				status: null,
				payment: null
			}
		},
		setOrderPromoCode(state: any, data: Promocode) {
			state.order.promoCodeId = data.id;
			state.orderPromocode = data;
		},
		resetOrderPromoCode(state: any, data: Promocode) {
			state.order.promoCodeId = undefined;
			state.orderPromocode = {...{
				id: 0,
				seriesNumber: '',
				promoCodeType:'',
				dateStart: undefined,
				dateEnd: undefined,
				numberOfPromocodes: undefined,
				amountOfDiscount: undefined,
				typeOfDiscount: '',
				cityId: undefined,
				isBadPromoCode: null
			}, ...data}
		},
		resetEmptyMenu(state: RootState) {
			state.order.seats.forEach(item => {
				if (item.menuId == -1) {
					item.menuId = undefined;
				}

				if (item.beverageId == -1) {
					item.beverageId = undefined;
				}
			});
		},
		setRegularTourEditingOrderState(state: RootState, order: Order): void {
			state.order.date = new Date(order?.tour?.departure ?? new Date());
			state.order.time = Time.fromDate(new Date(order?.tour?.departure ?? new Date()));
			state.order.guestCount = order.seats.length;
			state.order.disabledGuestCount = order.disabledGuestCount;
			state.order.guestsWithDisabilities = order.disabledGuestCount > 0;
			state.order.promoCode = (<Promocode><unknown>order.promoCode)?.seriesNumber;
			state.order.certificateNumber = order.giftCertificate?.number!;
			state.order.tourId = order.tourId;
			state.selectionModel.selection.selectedObjects = order.seats.map(seat => ({
				id: seat.seatId,
				type: BusObjectTypes.Seat
			}));
			
			order.seats.forEach(s => s.isAllergy = s.allergyId ? true : false);

			state.menuInfo.menus.forEach(menu => {
				if (!state.order.menus.find(x => x.menuId === menu.id)) {
					state.order.menus.push(<OrderMenu>{ menuId: menu.id, amount: 0 });
				}
			});
			
			state.menuInfo.menus.forEach(beverage => {
				if (!state.order.beverages.find(x => x.beverageId === beverage.id)) {
					state.order.beverages.push(<OrderBeverage>{ beverageId: beverage.id, amount: 0 });
				}
			});		
		},
		setPrivateHireOrderEditingOrderState(state: RootState, order: Order): void {
			const orderPH = new OrderPrivateHire()
			const tourPH = order?.tour?.privateHire

			orderPH.date = new Date(order?.tour?.departure ?? new Date())
			orderPH.date = new Date(orderPH.date.getTime() - orderPH.date.getTimezoneOffset() * 60 * 1000)

			orderPH.timeFrom = Time.fromDate(new Date(order?.tour?.departure ?? new Date()))
			orderPH.timeTo = Time.fromDate(new Date(order?.tour?.arrival ?? new Date()))
			orderPH.isAllDay = orderPH.timeFrom.equals(new Time(0,0,0)) && orderPH.timeTo.equals(new Time(23,59,59))
			orderPH.blockBookingForDraft = tourPH?.blockBookingForDraft ?? false
			orderPH.blockBookingTimeFrom = Time.fromDate(new Date(tourPH?.blockBookingDateFrom ?? new Date()))
			orderPH.blockBookingTimeTo = Time.fromDate(new Date(tourPH?.blockBookingDateTo ?? new Date()))
			orderPH.departurePoint = tourPH?.departurePoint ?? null
			orderPH.arrivalPoint = tourPH?.arrivalPoint ?? ''
			orderPH.routeId = order?.tour?.routeId ?? null
			orderPH.stops = tourPH?.stops ?? []
			orderPH.tourPrice = tourPH?.price ?? 0
			orderPH.busId = order?.tour?.busId ?? 0
			state.order.guestCount = tourPH?.guestCount ?? 0

			state.order.privateHire = {...state.order.privateHire, ...orderPH}
		},
		setCalculationCostTour(state: RootState, data: CalculationCostTourResponse): void {
			state.calculationCostTour = data;
		},
		setUpgradeMode(state: RootState, data: Boolean): void {
			state.isUpgradeMode = data;
		},
		setPaymentFromOrderClient(state: RootState, data: Boolean): void {
			const client = state.order?.client;
			if (client && state.payment) {
				state.payment.email = client.email;
				state.payment.repeatemail = client.email;
				state.payment.name = client.fullName ?? '';
				state.payment.phone.number = client.phoneNumber ?? '';
			}
		}
	}
	
	export const getters: GetterTree<RootState, RootState> = {
		seatPrice: (state, getters) => (seatId: number): number => {
			if (state.schemeTables?.length) {
				return state.schemeTables
							.find(x => x.seats.some(x => x.id == seatId))?.seats
							.find(x => x.id == seatId)?.price ?? 0;
			}
			else {
				const table = getters.tables.find((t: Table) => t.seats.some((s: Seat) => s.id == seatId))
				const tour = state.order.tour
				if (table && tour?.vipPrice && tour?.seatPrice) {
					return table.isVip ? tour.vipPrice : tour.seatPrice
				}
			}
			return 0;
			//return getters.seats.find((x: Seat) => x.id == seatId)?.price ?? 0;
		},
		seatFullName(state, getters) {
			return function(seatId: number, translate: any): string {
				const table = getters.tables.find((x: Table) => x.seats.some(y=>y.id===seatId));
				if(table) {
					const seat = table.seats.find((x: Seat) => x.id == seatId) as Seat;
					if (seat.type == SeatType.Disabled) {
						// @ts-ignore
						return this.$i18n.t('booking.wheelChair')
					} else {			
						return `${table.number} ${seat.name}`;
					}
				}
				return '';				
			}
		},
		totalPrice(state, getters): number {
			const seatCost = state.order.seats
				.map((item) => 1 * getters.seatPrice(item.seatId))
				.reduce((acc, item) => acc + item, 0);
	
			const menuCost = state.order.menus
				.filter(x => x.menuId && x.amount)
				.map((item) => item.amount * state.menuInfo.menus.filter((i) => i.id === item.menuId)[0].price)
				.reduce((acc, item) => acc + item, 0);
	
			const beverageCost = state.order.beverages
				.filter(x => x.beverageId && x.amount)
				.map((item) => item.amount * state.menuInfo.beverages.filter((i) => i.id === item.beverageId)[0].price)
				.reduce((acc, item) => acc + item, 0);
	
			const surpriseCost = state.order.surprises
				.map((item) => item.amount * state.menuInfo.surprises.filter((i) => i.id === item.surpriseId)[0].price)
				.reduce((acc, item) => acc + item, 0);

			const privateHireCost = state.order?.privateHire?.tourPrice ?? 0;
	
			return seatCost + menuCost + beverageCost + surpriseCost + privateHireCost;
		},
		seatTotalPrice(state, getters): number {
			return state.order.seats
				.map((item) => 1 * getters.seatPrice(item.seatId))
				.reduce((acc, item) => acc + item, 0);
		},
		extrasTotalPrice(state, getters): number {
			const menuCost = state.order.menus
			.filter(x => x.menuId && x.amount)
			.map((item) => item.amount * state.menuInfo.menus.filter((i) => i.id === item.menuId)[0].price)
			.reduce((acc, item) => acc + item, 0);

			const beverageCost = state.order.beverages
				.filter(x => x.beverageId && x.amount)
				.map((item) => item.amount * state.menuInfo.beverages.filter((i) => i.id === item.beverageId)[0].price)
				.reduce((acc, item) => acc + item, 0);

			return beverageCost + menuCost;
		},
		totalVat(state, getters): number {
			return getters.totalPrice * 0.20;
		},	
		availableTimes(state): string[] {
			return state.routeInfo.tours
				// .filter((item) => item.departure.split("T")[0] === state.order.date)
				.map((item) => item.departure.split("T")[1]);
		},
		availableDate(state): string[] {
			var dates = state.routeInfo.tours
			.map((item) => item.departure.split("T")[0]);
	
			return  [...new Set(dates)];
		},
		tables(state): Table[] {
			const tours = state.routeInfo.tours
				.filter((item) => item.id == state.order.tourId);

			let res = [] as Table[];

			if (state.buses.length) {
				res = state.buses[0].tables;
			} else if (tours && tours.length > 0) {
				res = tours[0].tables;
			}
			return res;
		},
		seats(state, getters): Seat[] {
			const tables = getters.tables;
			return tables
			.map((item: Table) => item.seats)
			.reduce((x: Seat[], y: Seat[]) => x.concat(y), []);			
		},		
		availableSeatCountForDisabled(state): number {
			let result = 0;
	
			const tours = state.routeInfo.tours
				.filter((item) => item.id == state.order.tourId);
	
			if (tours && tours.length > 0) {
				const tablesForDisabled = tours[0].tables.filter((item) => item.category.id === TableCategoryEnum.Wheelchair);
	
				if (tablesForDisabled && tablesForDisabled.length > 0) {
					result = tablesForDisabled
						.map((item) => item.seats.length)
						.reduce((acc, item) => acc + item, 0);
				}
			}
	
			return result;
		},
		order(state): Order {
			return state.order;
		},
		route(state): Route {
			return state.routeInfo.route;
		},
		tours(state): Tour[] {
			return state.routeInfo.tours;
		},
		calculationCostTour(state): CalculationCostTourResponse {
			return state.calculationCostTour;
		},
		isUpgradeMode(state): Boolean {
			return state.isUpgradeMode;
		},
		schemeTables(state): SchemeTableModel[] {
			return state.schemeTables;
		},
		orderInfo(state, getters): OrderInfo {

			const busTables = getters.tables as Table[]
			const busSeats  = getters.seats as Seat[]

			const infoTables = [] as OrderInfoTable[];

			state.order.seats.forEach(s => {

				const seat = busSeats.find(x => x.id == s.seatId)

				if (seat) {

					const table = busTables.find(x => x.id == seat.tableId)

					if (table) {
						let infoTable = infoTables.find(x => x.id == table.id)

						if (!infoTable) {
							infoTable = { id: table.id, x: table.x, y: table.y, seats: []  }
							infoTables.push(infoTable)
						}

						infoTable.seats.push({  
							id: seat.id,
							x: seat.x,
							y: seat.y,
							isSelected: true						
						})
					}
				}
			})

			infoTables.forEach(it => {
				it.seats = [...it.seats, ...busSeats
				.filter(x => x.tableId == it.id)
				.filter(x=> !it.seats.find(s => s.id == x.id))
				.map(x => {
				return {
					id: x.id,
					x: x.x,
					y: x.y,
					isSelected: false					
				}})]
			})

			return {
				guests: [],
				tables: infoTables
			}
		}
	}

export default {
	state,
	actions,
	mutations,
	getters
}