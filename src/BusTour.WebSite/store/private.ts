import { ActionTree, GetterTree, MutationTree } from 'vuex';
import config from "@/config";
import { Roles, RootStatePrivate, User } from "@/types/private";

export const state = (): RootStatePrivate => ({
  users: [],
  userToAdd: {
    id: 0,
    userName: "",
    role: "",
    password: "",
    retypePassword: "",
  },
  userToUpdate: {
    id: 0,
    userName: "",
    role: "",
    password: "",
  },
  usernameErrorMessage: "",
  currentAction: "",
});

interface Actions<S, R> extends ActionTree<S, R> { }

export const actions: Actions<RootStatePrivate, RootStatePrivate> = {
  async getUsers({ commit }) {
    try {
      const response = await this.$axios.$get<User[]>(config.bustourApiUrl + "/User");

      commit("setUsers", response);
    }
    catch (e) {
      console.log("getUsers threw an exception: " + e);
    }
  },

  async addUser({ state, dispatch, commit }) {
    try {
      commit("setCurrentAction", "add");
      commit("setUsernameErrorMessage", "");

      const response = await this.$axios.$post<User>(config.bustourApiUrl + "/User", state.userToAdd)
        .then(() => {
          commit("setUserToAddData", <User>{
            id: 0,
            userName: "",
            password: "",
            role: "",
          });
        })
        .catch((e) => {
          commit("setUsernameErrorMessage", e.response.data.message);
        });

      await dispatch("getUsers");
    }
    catch (e) {
      console.log("addUser threw an exception: " + e);
    }
  },

  async deleteUser({ dispatch }, userId) {
    try {
      await this.$axios.$delete(config.bustourApiUrl + `/User/${userId}`);

      await dispatch("getUsers");
    }
    catch (e) {
      console.log("deleteUser threw an exception: " + e);
    }
  },

  async updateUser({ state, dispatch, commit }) {
    try {
      commit("setCurrentAction", "update");
      commit("setUsernameErrorMessage", "");

      await this.$axios.$put(config.bustourApiUrl + `/User/${state.userToUpdate.id}`, state.userToUpdate)
      .then()
      .catch((e) => {
        commit("setUsernameErrorMessage", e.response.data.message);
      });;

    await dispatch("getUsers");
    }
    catch (e) {
      console.log("deleteUser threw an exception: " + e);
    }
  }
}

export const mutations: MutationTree<RootStatePrivate> = {
  setCurrentAction(state: RootStatePrivate, payload: string,) {
    state.currentAction = payload;
  },

  setUsernameErrorMessage(state: RootStatePrivate, payload: string,) {
    state.usernameErrorMessage = payload;
  },

  setUsers(state: RootStatePrivate, payload: User[]) {
    state.users = payload;
  },

  setUserToAddData(state: RootStatePrivate, payload: User) {
    state.userToAdd = payload;
  },

  setUserToUpdateData(state: RootStatePrivate, payload: User) {
    state.userToUpdate = payload;
  },
}

export const getters: GetterTree<RootStatePrivate, RootStatePrivate> = {
}

export default {
  state,
  actions,
  mutations,
  getters
}