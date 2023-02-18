export class CustomValidator {
	constructor() {
        items: null
    }

    init(parameters) {
        this.destroy();

        if(parameters.rules == null){
            return;
        }

        this.items = parameters.rules;
    }

    destroy() {
        if(this.items !== null) {
            this.items = null;
        }
    }

    validate() {
        if (this.items === null) {
            return true;
        }

        let self = this;
        let errors = [];
        _.each(this.items, function (item) {
            if(item.required) {
                self._validateRequired(item, errors);
            } else if(item.dropDownRequired) {
                self._validateDropDownRequired(item, errors);
            }
        })

        return errors;   
    }

    _validateRequired(item, errors) {
        if(item.el.elValue === undefined /*|| isNaN(item.el.elValue)*/ || item.el.elValue == null || item.el.elValue === ''){
            this._addError(item, errors);
        } else {
            this._removeError(item);
        }
    }

    _validateDropDownRequired(item, errors) {
        if (item.el.elValue === undefined /*|| isNaN(item.el.elValue)*/ || item.el.elValue == null || 
            item.el.elValue === -1 || item.el.elValue === '-1' || item.el.elValue === ''){
            this._addError(item, errors);
        } else {
            this._removeError(item);
        }
    }

    _addError(item, errors) {
        errors.push({
            key: item.name,
            text:item.message
        });
        
        $(item.el.$el).find('[name="'+item.name+'"]').closest('div').removeClass('has-error').addClass('has-error');
    }
    
    _removeError(item) {
        $(item.el.$el).find('[name="'+item.name+'"]').closest('div').removeClass('has-error');
    }
}