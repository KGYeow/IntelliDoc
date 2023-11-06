<script setup lang="ts">
const primary = ref('rgb(var(--v-theme-primary))');
const secondary = ref('rgb(var(--v-theme-secondary))');
</script>
<template>
  <div class="logo">
    <NuxtLink to="/">
      <img src="images/logos/usmLogo.png" height="40">
    </NuxtLink>
  </div>
</template>