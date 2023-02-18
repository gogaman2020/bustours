import { mapState, Mapper, Computed, InlineComputed } from 'vuex';

function deepClone(obj: any): any {
    if (typeof obj == 'function') {
      return obj;
    }
    var result: any = Array.isArray(obj) ? [] : {};
    for (var key in obj) {
      var value = obj[key];
      var type = {}.toString.call(value).slice(8, -1);
      if (type == 'Array' || type == 'Object') {
        result[key] = deepClone(value);
      } else if (type == 'Date') {
        result[key] = new Date(value.getTime());
      } else {
        result[key] = value;
      }
    }
    return result;
}

interface PropsMapper<R> {
  <Key extends string>(map: Key[], namespace: string, state: string, mutation?: string, ex?: string): { [K in Key]: R};
  <Map extends Record<string, string>>(map: Map, namespace: string, state: string, mutation?: string, ex?: string): { [K in keyof Map]: R };
}

export let mapProps: PropsMapper<Computed>;

mapProps = function(props: string[] | Record<string, string>, namespace: string, state: string, mutation?: string, ex?: string) {

  mutation = mutation || state;
    const stateSplit = state.split('.')
    state = stateSplit[stateSplit.length - 1]    

    let map: Record<string, string> = {}
    if (Array.isArray(props)) {
        props.forEach(x => map[x] = x);
    } else {
        map = props;
    }

    let computed = Object.keys(map).reduce((obj: any, prop: string) => {
        obj[prop] = {
            get(): any {
                return state ? this[state][map[prop]] : this[map[prop]]
            },
            set(value: any) {
                this.$store.commit(`${namespace}/${mutation}`, { [map[prop]]: value }); 
            }
        }
        return obj
    }, {})

    computed = {...computed, ...mapState(
      namespace, 
      { [state]:  (storeState: any) => deepClone(stateSplit.reduce((ss, spl) => ss[spl], storeState)) })
    };

    return computed;
}