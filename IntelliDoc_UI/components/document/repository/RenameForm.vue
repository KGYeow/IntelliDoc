<template>
  <form @submit.prevent="renameDoc">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col>
            <v-label class="text-caption">Document Name</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="renameDocInfo.nameWithoutExt.value"
              :suffix="renameDocInfo.extension"
              :error-messages="renameDocInfo.nameWithoutExt.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">  
      <v-btn color="primary" type="submit">Submit</v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const props = defineProps({
  docId: Number,
  docNameOld: String,
})
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  initialValues: {
    nameWithoutExt: props.docNameOld.slice(0, props.docNameOld.lastIndexOf(".")),
  },
  validationSchema: {
    nameWithoutExt(value) {
      return value ? true : 'Document name is required'
    }
  }
})
const renameDocInfo = ref({
  nameWithoutExt: useField('nameWithoutExt'),
  extension: props.docNameOld.slice(props.docNameOld.lastIndexOf(".")),
})

// Methods
const renameDoc = handleSubmit(async(values) => {
  try {
    const result = await useFetchCustom.$put(`/Repository/Rename/${props.docId}/${values.nameWithoutExt}${renameDocInfo.value.extension}`)
    
    if (!result.error) {
      emit('close-modal', false)
      renameDocInfo.value.nameWithoutExt.resetField()
      renameDocInfo.value.extension = null
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>