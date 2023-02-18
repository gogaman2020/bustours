declare module '*.vue' {
  import Vue from 'vue'
  export default Vue
}

declare module '*.scss' {
    const content: { [className: string]: string };
    export default content;
}

declare module 'vue-numeric-input' {
  const content: {[className: string]: any}
  export default content
}

declare var JsonExcel: any;
declare module "vue-json-excel" {
  export = JsonExcel;
}