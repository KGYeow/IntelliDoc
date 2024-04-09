<template>
  <form @submit.prevent="emptyArchive">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col>
            <v-alert class="p-0" icon="$warning" title="Delete Forever?" variant="text" prominent>
              <div class="pt-2">
                All documents in the archive will be deleted forever and you won't be able to restore them.
              </div>
            </v-alert>
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">
      <v-btn color="primary" variant="outlined" @click="emit('close-modal', false)">Cancel</v-btn>
      <v-btn color="danger" variant="tonal" type="submit">Delete Forever</v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
// Properties, Emit & Model
const emit = defineEmits(['close-modal'])

// Methods
const emptyArchive = async() => {
  try {
    const result = await useFetchCustom.$delete("/Archive/Delete/All")
    if (!result.error) {
      emit('close-modal', false)
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
</script>