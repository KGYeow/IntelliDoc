<template>
  <v-locale-provider>
    <v-app>
      <LayoutFullMain/>
      <v-main>
        <v-container fluid class="page-wrapper">
          <div class="maxWidth">
            <RouterView/>
          </div>
        </v-container>
      </v-main>
      <!------Footer-------->
      <LayoutCustomFooter/>
    </v-app>
  </v-locale-provider>
</template>