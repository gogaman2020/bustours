<template>
	<div class="noPrint" :class="style.header">
		<div :class="style.headerLogo">
			<nuxt-link :to="localePath('/')">PRIME Bus Tours</nuxt-link>
		</div>

		<div :class="style.headerBurgerMenuIcon" @click="toggleMenuVisible"></div>

		<div :class="style.headerMenu">
			<div :class="[style.headerMenuBackdrop, style.dN]" @click="toggleMenuVisible"></div>
			<div :class="[style.headerMenuWrapper, style.headerBurgerMenu]">
				<div :class="[style.headerBurgerMenuAuthControl, style.headerItem]">
					<auth-control />
				</div>

				<div :class="style.headerItem" v-for="item of menuItems" :key="item.textKey" @click="toggleMenuVisible($event, false)">
					<nuxt-link 
					:class="{'active-link': isRouteActive(item.relativePath) }" 
					:to="{ path: localePath(item.relativePath), hash: item.useHash ? hash() : '' }">
						{{ $t(item.textKey) }}
					</nuxt-link>
				</div>

				<div :class="[style.headerBurgerMenuLangSwitcher, style.headerItem]">
					<lang-switcher @lang-selected="toggleMenuVisible($event, false)" />
				</div>
			</div>
		</div>

		<div v-if="!isLoggedIn" :class="style.headerRightMenu">
			<div :class="[style.headerItem, style.bookingLink]">
				<nuxt-link 
					:to="{path: localePath(`/${rightMenuConfig.get(currentPage).path}`), hash: rightMenuConfig.get(currentPage).useHash ? hash() : '' }"
				>
					<bb-button :hover-effect="false" :class="style.headerRightMenuButton" style="padding-top: 1px;">
						
							{{ $t(`menu.${rightMenuConfig.get(currentPage).path}`) }}
						
					</bb-button>
				</nuxt-link>
			</div>
			<lang-switcher />
		</div>
		<auth-control v-else :class="style.headerAuthControl" />
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { Roles } from "~/types/private";
import style from "./style.module.scss";
import path from "path";

import langSwitcher from "../lang-switcher/lang-switcher.vue";
import authControl from "../auth-control/auth-control.vue";
import bbButton from "~/components/controls/bb-button/bb-button.vue";

enum MenuConfigType {
	ByAuth,
	ByPage,
}

interface MenuConfigParams {
	currentPage: string
	isLoggedIn: boolean,
	userRole: Roles
}

export default Vue.extend({
	name: "layout-header",

	components: {
		langSwitcher,
		authControl,
		bbButton,
	},

	props: {
		isLoggedIn: Boolean,
		userRole: String,
	},

	data() {
		return {
			style,
			isMobileMenuVisible: false,
			menuItemsConfig: { 
				notRequiringAuth: [
					{
						textKey: "menu.onBoard",
						relativePath: "/on-board"
					}, {
						textKey: "menu.itineraries",
						relativePath: "/itineraries"
					}, {
						textKey: "menu.privateHire",
						relativePath: "/private-hire"
					}, {
						textKey: "menu.gifts",
						relativePath: "/add-gift-certificate",
						useHash: true
					}, {
						textKey: "menu.contactUs",
						relativePath: "/contact-us"
					}
				],
				requiringAuth: [
					{
						textKey: "menu.currentBookings",
						relativePath: "/current-bookings",
						requiredRole: null
					}, {
						textKey: "menu.orders",
						relativePath: "/tour-fb",
						requiredRole: null
					}, {
						textKey: "menu.newBooking",
						relativePath: "/new-booking",
						requiredRole: [Roles.Administrator, Roles.Supervisor],
						useHash: true
					}, {
						textKey: "menu.promoCodes",
						relativePath: "/promocodes",
						requiredRole: [Roles.Administrator, Roles.Supervisor]
					}, {
						textKey: "menu.gifts",
						relativePath: "/gift-certificates",
						requiredRole: [Roles.Administrator, Roles.Supervisor]
					}, {
						textKey: "menu.toursManagement",
						relativePath: "/tours-management",
						requiredRole: [Roles.Supervisor]
					}, {
						textKey: "menu.newTours",
						relativePath: "/create-tour",
						requiredRole: [Roles.Administrator, Roles.Supervisor]
					}, {
						textKey: "menu.private",
						relativePath: "/private",
						requiredRole: [Roles.Supervisor]
					},  
				],
				pages: {
					authorization: []
				},
				get(mode: MenuConfigType, params: MenuConfigParams) {
					let config;

					if (mode == MenuConfigType.ByAuth) {
						const { userRole, isLoggedIn} = params; 

						config = isLoggedIn 
							? this.requiringAuth.filter(item => !item.requiredRole || item.requiredRole.includes(userRole as Roles) )
							: this.notRequiringAuth;	
					} else if (mode == MenuConfigType.ByPage) {
						const { currentPage } = params;

						config = (this.pages as any)[currentPage];
					}

					return config;
				},
			},
			rightMenuConfig: {
				config: [
					{
						condition: (page: string) => page != "authorization",
						path: 'booking',
						useHash: true
					},
					{
						condition: (page: string) => page === "authorization",
						path: 'logIn',
					}
				],
				get(page: string) {
					return this.config.find(x => x.condition(page))
				}
			}
		}
	},

	computed: {
		currentPage(): string {
			return this.parsePath(this.$route.fullPath);
		},
		menuItems(): any {
			const mode: MenuConfigType = this.doesCurrentPageExistInConfig() 
				? MenuConfigType.ByPage
				: MenuConfigType.ByAuth;

			const config: MenuConfigParams = {
				currentPage: this.currentPage,
				isLoggedIn: this.isLoggedIn,
				userRole: (this.userRole as Roles),
			} 

			return this.menuItemsConfig.get(mode, config);
		}
	},

	watch: {
		isMobileMenuVisible(newValue: boolean, oldValue: boolean) {
			const method = newValue ? "remove" : "add";

			document.querySelector(`.${this.style.headerMenuWrapper}`)?.classList[method](this.style.headerBurgerMenu);
			document.querySelector(`.${this.style.headerMenuBackdrop}`)?.classList[method](this.style.dN);
		}
	},

	methods: {
		hash() {
			return '#' + new Date().getTime().toString();
		},
		toggleMenuVisible(event: Event, value: boolean) {
			value = value != undefined ? value : !this.isMobileMenuVisible;
			
			this.isMobileMenuVisible = value;
		},
		parsePath(fullPath: string): string {
			return path.isAbsolute(fullPath)
				? fullPath.replace(/\//g, "")
				: fullPath;
		},
		doesCurrentPageExistInConfig() {
			return this.menuItemsConfig.pages.hasOwnProperty(this.currentPage);
		},
		isRouteActive(path: string) {
			const currentSplit = this.$nuxt.$route.path.split('/')
			const pathSplit = path.split('/')
			if (currentSplit.length <= pathSplit.length + 1 && currentSplit[currentSplit.length-1] == pathSplit[pathSplit.length-1]) {
				return true;
			} else {
				return false;
			}
		}
	},
})
</script>