export const required = (value: any): boolean => value === 0 || !!value

export const emailValidator = (value: string): boolean => {
    if (value) {
        const spl1 = value.split('@');
        if (spl1.length == 2 && spl1[0].length >= 2) {
            const spl2 = spl1[1].split('.');
            return spl2.length == 2 && spl2[0].length >= 2 && spl2[1].length >= 2;
        }
        return false; 
    } 
    return true;
}

export const cardNumberValidator = (value: any): boolean =>  !!value && value.length == 19