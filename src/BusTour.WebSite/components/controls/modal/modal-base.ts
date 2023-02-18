import Vue from "vue";
import { Action } from "@/types/common";

interface IModalController {
	close: Action<boolean>
}

const MODAL_CONTROLLER: IModalController = {
	close: () => true
};

export default Vue.extend({
	data() {
		return {
			modalVisible: false,
			modalBody: "",
			modalTitle: "Alert",
		}
	},

	methods: {
		async open(modalBody?: string, modalTitle?: string): Promise<boolean> {
			const modalPromise = new Promise<boolean>(resolve => {
				MODAL_CONTROLLER.close = resolve;
			});

			this.modalVisible = true;
			this.modalBody = modalBody || this.modalBody;
			this.modalTitle = modalTitle || this.modalTitle;

			return modalPromise;
		},
		close(answer: boolean): void {
			this.modalVisible = false;
			MODAL_CONTROLLER.close(answer);
		}
	}
});