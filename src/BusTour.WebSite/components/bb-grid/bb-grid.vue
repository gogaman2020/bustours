<template>
	<div :class="[s.root, maxHeight == 'auto' ? s.noScroll : '']" :style="{ maxHeight: maxHeight }">
		<table v-cloak :class="s.bbGrid" :style="customStyles">
			<thead>
				<tr :style="{ background: background }">
					<slot></slot>
				</tr>
			</thead>

			<tbody>
				<template v-if="grouping">
					<template v-for="group in groupedData">
						<tr :key="group.key" :class="s.bbGridGroupingRow">
							<td :colspan="customColumnSequence.length">
								<div v-if="grouping.template" v-html="grouping.template(group.key)"></div>
								<div v-else>{{ group.key }}</div>
							</td>
						</tr>
						<tr v-for="item in group.items" :class="s.bbGridRow" :key="item.uid">
							<td
								v-for="field in customColumnSequence" 
								:key="field"
								:width="computeColumnWidth(field)"
								:class="[s.bbGridColumn].concat(computeColumnVariants(field)).concat(getColumnClass(field))"
							>
								<slot 
									v-if="isNotEditable(field) || isNotOpenForEditing(item)"
									:name="field" 
									:row="item"
								>
									{{ item[field] }}
								</slot>
								<slot 
									v-else 
									:name="`${field}:Editor`" 
									:syncCallback="item.syncCallback.bind(item, field)"
									:applyAmendment="item.applyAmendment.bind(item)"
									:value="item[field]"
									:row="item"
								>
									<TextInput variant="view" :value="item[field]" @input="item.syncCallback(field, $event)" />
								</slot>
							</td>
						</tr>
					</template>
				</template>
				<template v-else>
					<tr v-for="item in pagingData" :class="s.bbGridRow" :key="item.uid">
						<td
							v-for="field in customColumnSequence" 
							:key="field"
							:width="computeColumnWidth(field)"
							:class="[s.bbGridColumn].concat(computeColumnVariants(field)).concat(getColumnClass(field))"
						>
							<slot 
								v-if="isNotEditable(field, item) || isNotOpenForEditing(item)" 
								:name="field" 
								:row="item"
							>
								{{ item[field] }}
							</slot>
							<slot 
								v-else 
								:name="`${field}:Editor`" 
								:syncCallback="item.syncCallback.bind(item, field)"
								:applyAmendment="item.applyAmendment.bind(item)"
								:value="item[field]"
								:row="item"
							>
								<TextInput variant="view" :value="item[field]" @input="item.syncCallback(field, $event)" />
							</slot>
						</td>
					</tr>
				</template>
			</tbody>

			<tfoot></tfoot>
		</table>

		<BbGridPager 
			v-if="pager && data.length" 
			:total="data.length" 
			:page-size="pageSize"
			:visible-page-count="visiblePageCount"
			:class="s.bbGridPager"
			@page="onPageChange" 
		/>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import s from "./s.module.scss";

import { IGridElement, IGridColumnProps, IPaging, IGroupable, GridRowEditingState } from "./bb-grid-types";
import { groupArrayItemsBy, keyGenerator } from "./bb-grid-utils";

import BbGridColumn from "./bb-grid-column/bb-grid-column.vue";
import BbGridPager from "./bb-grid-pager/bb-grid-pager.vue";

import TextInput from "@/components/controls/text-input/text-input.vue";

const uidGen = keyGenerator();
const fieldKeyGen = keyGenerator();

export default Vue.extend({
	components: {
		BbGridColumn,
		BbGridPager,

		TextInput,
	},

	props: {
		data: {
			type: Array,
			default: () => [] as [],
		},
		pager: {
			type: Boolean,
			default: true,
		},
		pageSize: {
			type: Number,
			default: 10,
		},
		visiblePageCount: {
			type: Number,
			default: 10,
		},
		grouping: {
			type: Object,
		},
		columnSpacing: {
			type: String,
			default: "0",
		},
		rowSpacing: {
			type: String,
			default: "10px",
		},
		maxHeight: {
			type: String,
			default: "auto"
		},
		background: {
			type: String,
			default: "white"
		}		
	},

	data() {
		return {
			el: null as Element | null,
			s,
			paging: {
				skip: 0,
				take: this.pageSize,
			} as IPaging,
			gridData: (this as any).prepareGridData(this.data) as IGridElement[],
			GridRowEditingState,
		}
	},

	watch: {
		data(newValue: any[]) {
			this.gridData = this.prepareGridData(newValue);
		}
	},

	created(): void {
		this.$parent.$on("bb-grid-edit-or-update-row", this.editOrUpdateRow);
	},

	mounted(): void {
		this.el = this.$el;	
	},

	computed: {
		groupedData(): IGroupable {
			return groupArrayItemsBy(this.pagingData, this.grouping.key);
		},
		pagingData(): any[] {
			if (!this.pager) {
				return this.gridData;
			}
	
			const { take, skip } = this.paging;

			return this.gridData.slice(skip, take);
		},
		customColumnSequence(): string[] {
			if (this.$slots.default == undefined) {
				return [];
			}

			let columns = this.$slots.default
				.filter(column => column.tag != undefined)
				.map(column => {
					const field = (column.componentOptions?.propsData as IGridColumnProps).field 
					?? 
					`BbGridAutoField${fieldKeyGen.next().value}`;

					(column as any).componentOptions.propsData.field = field;

					return field;
				})
				.filter(field => field != "");
			
			return columns;
		},
		columnProps(): IGridColumnProps[] {
			if (this.$slots.default == undefined) {
				return [];
			}

			return this.$slots.default
				.filter(column => column.tag != undefined)
				.map(column => {
					const props = column?.componentOptions?.propsData as IGridColumnProps;

					props.width = props.width ?? "auto";
					props.editable = props.editable ?? false;

					return props;
				});
		},
		autoColumnsSize(): string {
			const fixedColumnsWidths = this.columnProps
				.filter(col => col.width != "auto")
				.map(col => col.width);

			const fixedTotalSize = fixedColumnsWidths.reduce((acc: number, curr: string) => acc + parseInt(curr), 0);
			const responsiveSize = this.gridWidth - fixedTotalSize;
			const autoColumnsLength = this.columnProps.filter(column => column.width == "auto").length;
			const autoColumnSize = `${responsiveSize / autoColumnsLength}px`;

			return autoColumnSize;
		},
		gridWidth(): number {
			if (!process.browser || !this.el) {
				return 0;
			}

			const styles = getComputedStyle(this.el);
			const pl = styles.getPropertyValue("padding-left");
			const pr = styles.getPropertyValue("padding-right");
			const columnsGap = (this.columnProps.length + 1) * parseInt(this.columnSpacing);
			const width = this.el.clientWidth - parseInt(pl) - parseInt(pr) - columnsGap;

			return width;
		},
		customStyles(): string {
			const styles = [
				`border-spacing: ${this.columnSpacing} ${this.rowSpacing}`,
				`margin: -${this.rowSpacing} -${this.columnSpacing}`
			];

			return styles.join(";");
		}
	},

	methods: {
		generateKey() {
			return uidGen.next().value;
		},
		computeColumnWidth(field: string): string {		
			const column = this.columnProps.find(column => column.field == field);

			if (!column) {
				return "";
			}

			if (column.width != "auto") {
				return column.width;
			}

			return this.autoColumnsSize;
		},
		getColumnTitle(field: string): string {		
			return this.columnProps.find(column => column.field == field)?.title ?? '';
		},		
		computeColumnVariants(field: string): string[] {		
			const column = this.columnProps.find(column => column.field == field);
			return column?.variants?.map((x: string) => this.s[x]) ?? [];
		},		
		getColumnClass(field: string): string {
			const column = this.columnProps.find(column => column.field == field);

			return column?.cellClass ?? "";
		},
		onPageChange(paging: IPaging): void {
			this.paging = paging;
		},
		prepareGridData(data: any[]): any[] {
			let keyObject: IGridElement;

			return data.map(el => {
				keyObject = { 
					uid: this.generateKey(),
					editingState: GridRowEditingState.Disabled,
					dirtyFields: {},
					isDirty: false,
					syncCallback(field: string, data: any): void {	
						this.dirtyFields[field] = data;
						this.isDirty = true;
						//this[field] = data;
					},
					applyAmendment(): void {
						Object
							.keys(this.dirtyFields)
							.forEach((key: string) => {
								if (this.hasOwnProperty(key)) {
									this[key] = this.dirtyFields[key];
								}
							});
					},
					clearDirtyFields(): void {
						this.dirtyFields = {};
						this.isDirty = false;
					}
				};
				
				return Object.assign({}, Object.assign(el, keyObject));
			});
		},
		getToggledEditingState(state: GridRowEditingState): GridRowEditingState {
			return state == GridRowEditingState.Enabled 
				? GridRowEditingState.Disabled
				: GridRowEditingState.Enabled;
		},
		editOrUpdateRow(row: any): void {
			const target = this.gridData.find((gridRow: IGridElement) => gridRow.uid == row.uid);
			
			if (!target) {
				return;
			}

			const state = this.getToggledEditingState(target.editingState);

			if (state == GridRowEditingState.Disabled) {
				target.applyAmendment();
				this.$emit("update", target);
				target.clearDirtyFields();
			}

			target.editingState = state;

		},
		isNotEditable(field: string, item: any): boolean {
			const props = this.columnProps.find(col => col.field == field);
			
			let isNotEditable = (props?.editable ?? false) == false;

			if (!isNotEditable && props?.editableCondition && item) {

				isNotEditable = !props.editableCondition(item);
			}

			return isNotEditable;
		},
		isNotOpenForEditing(item: any): boolean {
			return item.editingState == GridRowEditingState.Disabled;
		},
	},
})
</script>