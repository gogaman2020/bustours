export interface Dictionary<T> {
  [Key: string]: T;
}

export enum Roles {
  User = "User",
  Administrator = "Administrator",
  Crew = "Crew",
  Supervisor = "Supervisor"
}

export enum AuthorityLevel {
  User = -1,
  Administrator = 1,
  Crew = 0,
  Supervisor = 2
}

export interface RootStatePrivate {
  users: User[];
  userToAdd: User;
  userToUpdate: User;
  usernameErrorMessage: string,
  currentAction: string,
}

export interface User {
  id: number;
  userName: string;
  role: Roles | string;
  password?: string;
  retypePassword?: "";
}