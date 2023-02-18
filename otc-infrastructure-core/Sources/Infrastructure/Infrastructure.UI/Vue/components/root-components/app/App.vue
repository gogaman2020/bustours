<template>
  <div class="content-template mt-3 row">
      <div class="col-md-8">
          <span>{{node}} {{test}} {{version}}</span>
      </div>
  </div>
</template>

<script>
export default {
        name: 'app',
        data: function() {
            return {
                node: process.env.NODE_ENV,
                version: process.env.VERSION,
                test: process.env.VUE_APP_TEST_VAR
            }
        }
}
</script>
