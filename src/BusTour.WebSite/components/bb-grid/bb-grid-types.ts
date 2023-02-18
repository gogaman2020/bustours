import { Action, Func } from "@/types/common";

export interface IGridElement extends Object {
	uid: number
	editingState: GridRowEditingState
	syncCallback: (field: string, data: any) => void
	applyAmendment: Action<void>
	clearDirtyFields: Action<void>
	dirtyFields: any
	isDirty: boolean
	[key: string]: any
}

export interface IGridColumnProps {
	title?: string
	field?: string
	width: string
	variants?: string[]
	editable?: boolean
	cellClass?: string
	editableCondition?: Func<any,boolean>
}

export interface IPagerPage {
	page: number
	isActive: boolean
}

export interface IPaging {
	skip: number
	take?: number | undefined
}

export interface IRange {
	min: number
	max: number
}

export interface IGroupable {
	[index: number]: IArrayGroup
}

export interface IArrayGroup {
	key: string
	items: any[]
}

export interface IGridGroup {
	key: Func<object, any>
	template?: Func<object, string>
}

export enum GridRowEditingState {
	Enabled,
	Disabled
}