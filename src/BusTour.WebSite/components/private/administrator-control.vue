<template>
	<div :class="s.container">
		<div :class="s.containerControl">
			<h1>{{ $t("admin.adminControl") }}</h1>
			<div :class="s.settingWrap">
				<div :class="s.settingItem">
					<div :class="s.settingName">{{ $t("admin.userName") }}</div>
					<input :class="s.settingInput" v-model="$v.newUserName.$model" @paste="validate" @keypress="validate"/>
					<div v-if="$v.newUserName.$error && $v.newUserName.$dirty" :class="s.validationMessage">This field is required</div>
					<div v-else-if="!$v.newUserName.$dirty && currentAction === 'add'" :class="s.validationMessage">{{usernameErrorMessage}}</div>
				</div>
				<div :class="s.settingItem">
					<div :class="s.settingName">{{ $t("admin.password") }}</div>
					<password-input :isPassword="false" :class="s.settingPassword" v-model="$v.newUserPassword.$model" />
					<div :class="s.validationMessage">{{$v.newUserPassword.$error && $v.newUserPassword.$dirty ? 'This field is required' : ' '}}</div>
				</div>
				<div :class="s.settingItem">
					<div :class="s.settingName">{{ $t("admin.role") }}</div>
					<drop-down :class="s.roleSelect" v-model="$v.newUserRole.$model" :items="roles"/>
					<div :class="s.validationMessage">{{$v.newUserRole.$error && $v.newUserName.$dirty ? 'This field is required' : ' '}}</div>
				</div>
				<div :class="s.settingItem">
					<div :class="s.settingName">{{ $t("admin.retypePassword") }}</div>
					<password-input :isPassword="false" :class="s.settingPassword" v-model="$v.newUserRetypePassword.$model" />
					<div v-if="$v.newUserRetypePassword.$error && $v.newUserRetypePassword.$dirty"
						:class="s.validationMessage"
					>This field is required</div>
					<div v-else-if="newUserPassword !== newUserRetypePassword && newUserPassword !== ''" :class="s.validationMessage">
						{{ $t("admin.retypePasswordWarning") }}
					</div>
				</div>
				<a :class="s.btnAdd">
					<button :class="s.btn" @click="addUser()">{{ $t("admin.add") }}</button>
				</a>
			</div>
		</div>
		<div :class="s.adminListWrap">
		<h3 :class="s.titleList">{{ $t("admin.userList") }}</h3>
			<div :class="s.containerList">
				<div :class="s.titleList">
					<div v-for="user in users" :key="user.id" :class="s.listRow">
						<template v-if="editUserId === user.id">
							<div :class="s.listItem">
								<input v-model="$v.updatedUserName.$model" :class="s.listInput"  @paste="validate" @keypress="validate"/>
								<div v-if="$v.updatedUserName.$error && $v.updatedUserName.$dirty" :class="s.validationMessage">This field is required</div>
								<div v-else-if="!$v.updatedUserName.$dirty && currentAction === 'update' " :class="s.validationMessage">{{usernameErrorMessage}}</div>
							</div>
							<div :class="s.listItem">
								<password-input
								v-model="updatedUserPassword"
								:class="s.listPassword"
								:isPassword="false"
								/>
							</div>
							<div :class="s.listItem">
								<drop-down :class="s.roleSelectWhite" v-model="updatedUserRole" :items="roles"/>
								</div>
								<div :class="[s.listItem, s.listItemActions]">
									<button :class="s.btn" @click="updateUser()">
									{{ $t("admin.update") }}
									</button>
									<button :class="s.btn" @click="deleteUser(user.id)">
									{{ $t("admin.delete") }}
									</button>
									<button :class="s.btn" @click="editUserId = -1">
									{{ $t("admin.cancel") }}
									</button>
								</div>
						</template>
						<template v-else>
							<div :class="s.listItem">
								<input v-model="user.userName" :class="s.listInput" readonly/>
							</div>
							<div :class="s.listItem">
								<input value="•••" :class="s.listInput" readonly/>
							</div>
							<div :class="s.listItem">
								<input v-model="user.role" :class="s.listInput" readonly/>
							</div>
							<div :class="[s.listItem, s.listItemActions]">
								<button :class="s.btn" @click="deleteUser(user.id)">
								{{ $t("admin.delete") }}
								</button>
								<button :class="s.btn" @click="setEditUser(user)">
								{{ $t("admin.edit") }}
								</button>
							</div>
						</template>
					</div>
				</div>
			</div>
		</div>
		<YesNoDialog ref="YesNoDialog">{{ $t("admin.deleteConfirm") }}</YesNoDialog>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import style from "./style.module.scss";
import passwordInput from "@/components/controls/password-input/password-input.vue";
import roleSelect from "@/components/controls/role-select/role-select.vue";
import { Roles, User } from "@/types/private";
import YesNoDialog from "~/components/controls/yes-no-dialog/YesNoDialog.vue";
import DropDown from "@/components/controls/dropdown/dropdown.vue";
import { SelectItem } from "@/types/common"

export default Vue.extend({
name: "administrator-control",
components: {
	passwordInput,
	roleSelect,
	YesNoDialog,
	DropDown,
},
data() {
	return {
	s: style,
	editUserId: -1,
	};
},
validations() {
		return {
			newUserName: {
				required(value: string): boolean {
					return !!value;
				}
			},
	newUserPassword: {
				required(value: string): boolean {
					return !!value;
				}
			},
	newUserRetypePassword: {
				required(value: string): boolean {
					return !!value;
				}
			},
	newUserRole: {
				required(value: string): boolean {
					return !!value;
				}
			},
	updatedUserName: {
		required(value: string): boolean {
					return !!value;
				}
	}
		}
	},
async created() {
	await this.$store.dispatch("private/getUsers");
},
computed: {
	roles(): SelectItem[] {
		return (Object.keys(Roles).filter(x => x !== 'User')
					.map(x => new SelectItem(x, x.toString())));
		}, 
	currentAction: {
	get(): string {
		return this.$store.state.private.currentAction;
	},
	},
	usernameErrorMessage: {
	get(): string {
		return this.$store.state.private.usernameErrorMessage;
	},
	},
	users: {
	get(): User[] {
		return this.$store.state.private.users;
	},
	},
	newUserName: {
	get(): string {
		return this.$store.state.private.userToAdd.userName;
	},
	set(val: string): void {
		this.$store.commit("private/setUserToAddData", <User>{
		id: 0,
		userName: val,
		password: this.newUserPassword,
		retypePassword: this.newUserRetypePassword,
		role: this.newUserRole,
		});
	},
	},
	newUserPassword: {
	get(): string {
		return this.$store.state.private.userToAdd.password;
	},
	set(val: string): void {
		this.$store.commit("private/setUserToAddData", <User>{
		id: 0,
		userName: this.newUserName,
		password: val,
		retypePassword: this.newUserRetypePassword,
		role: this.newUserRole,
		});
	},
	},
	newUserRetypePassword: {
	get(): string {
		return this.$store.state.private.userToAdd.retypePassword;
	},
	set(val: string): void {
		this.$store.commit("private/setUserToAddData", <User>{
		id: 0,
		userName: this.newUserName,
		password: this.newUserPassword,
		retypePassword: val,
		role: this.newUserRole,
		});
	},
	},
	newUserRole: {
	get(): Roles {
		return this.$store.state.private.userToAdd.role;
	},
	set(val: Roles): void {
		this.$store.commit("private/setUserToAddData", <User>{
		id: 0,
		userName: this.newUserName,
		password: this.newUserPassword,
		retypePassword: this.newUserRetypePassword,
		role: val,
		});
	},
	},
	updatedUserName: {
	get(): string {
		return this.$store.state.private.userToUpdate.userName;
	},
	set(val: string): void {
		this.$store.commit("private/setUserToUpdateData", <User>{
		id: this.editUserId,
		userName: val,
		password: this.updatedUserPassword,
		role: this.updatedUserRole,
		});
	},
	},
	updatedUserPassword: {
	get(): string {
		return this.$store.state.private.userToUpdate.password;
	},
	set(val: string): void {
		this.$store.commit("private/setUserToUpdateData", <User>{
		id: this.editUserId,
		userName: this.updatedUserName,
		password: val,
		role: this.updatedUserRole,
		});
	},
	},
	updatedUserRole: {
	get(): Roles {
		return this.$store.state.private.userToUpdate.role;
	},
	set(val: Roles): void {
		this.$store.commit("private/setUserToUpdateData", <User>{
		id: this.editUserId,
		userName: this.updatedUserName,
		password: this.updatedUserPassword,
		role: val,
		});
	},
	},
},
methods: {
	validate(event: any) {
		let regex = /[A-Za-z0-9\-\_\.]/;
		
		if(event.type === 'keypress' && !regex.test(event.key)) {
			event.preventDefault();	
		}

		if(event.type === 'paste') {
			let array: string[] = event.clipboardData.getData('text').split('');
			array.forEach(item => {
				if(!regex.test(item)) {
					event.preventDefault();
					return;
				}
			})
		}
	},
	async addUser() {
		this.$v.$touch();

		if (this.$v.newUserName.$invalid || 
			this.$v.newUserRole.$invalid ||
			this.$v.newUserPassword.$invalid || 
			this.$v.newUserRetypePassword.$invalid) {
					return;
				}
		else if(this.newUserPassword !== this.newUserRetypePassword) {
			return;
		}

		await this.$store.dispatch("private/addUser");
		
		this.$v.$reset();
		},
		async deleteUser(userId: number) {
		const answer = await ((this.$refs.YesNoDialog as any).open() as Promise<boolean>);
		
		if (answer) {
			await this.$store.dispatch("private/deleteUser", userId);
		}
	},
	async updateUser() {
		this.$v.$touch();

		if (this.$v.updatedUserName.$invalid) {
					return;
				}

		await this.$store.dispatch("private/updateUser");

		this.$v.$reset();

		if(!this.usernameErrorMessage) {
			this.editUserId = -1;

			this.$store.commit("private/setUserToUpdateData", <User>{
			id: 0,
			userName: "",
			role: Roles.User,
			password: "",
			});
		}
	},
	setEditUser(user: User) {
		this.editUserId = user.id;

		this.$store.commit("private/setUsernameErrorMessage", "");

		this.$store.commit("private/setUserToUpdateData", user);
	},
},
});
</script>