<template>
  <v-card elevation="10" class="withbg overflow-hidden">
    <v-card-item>
      <div class="d-flex align-center justify-space-between pt-sm-2">
        <v-card-title class="text-h5">Total Documents</v-card-title>
      </div>
      <v-row class="pt-2">
        <v-col cols="12">
          <v-sheet color="grey200" rounded>
            <v-row class="m-0">
              <v-col cols="6">
                <v-card-title class="text-subtitle-1 py-0">Stored</v-card-title>
                <v-card-title class="text-h4 py-0">
                  <v-tooltip :text="`Total Stored Documents: ${props.totalStoredDoc}`" activator="parent" location="top" offset="5"/>
                  {{ props.totalStoredDoc }}
                </v-card-title>
              </v-col>
              <v-col cols="6">
                <v-card-title class="text-subtitle-1 py-0">Archived</v-card-title>
                <v-card-title class="text-h4 py-0">
                  <v-tooltip :text="`Total Archived Documents: ${props.totalArchivedDoc}`" activator="parent" location="top" offset="5"/>
                  {{ props.totalArchivedDoc }}
                </v-card-title>
              </v-col>
            </v-row>
          </v-sheet>
        </v-col>
      </v-row>
    </v-card-item>
  </v-card>
</template>

<script setup>
// Properties
const props = defineProps({
  totalStoredDoc: { type: Number },
  totalArchivedDoc: { type: Number },
})
</script>