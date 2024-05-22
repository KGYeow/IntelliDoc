<template>
  <form @submit.prevent="updateDoc">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col>
            <v-label class="text-caption">Document</v-label>
            <v-file-input
              variant="outlined"
              density="compact"
              prepend-icon=""
              clear-icon="mdi-close-circle-outline fs-5"
              :accept="acceptedDocInput"
              :error-messages="editAttachmentDetails.attachment.errorMessage"
              v-model:model-value="editAttachmentDetails.attachmentInfo"
              @update:model-value="uploadFile"
              messages="Document files with a size less than 1GB"
              hide-details="auto"
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">
      <v-btn color="primary" type="submit" :disabled="loading">
        <v-progress-circular class="me-2" color="primary" :size="18" :width="3" indeterminate v-if="loading"/>
        {{ loading ? 'Updating' : 'Update' }}
      </v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const props = defineProps({
  docId: Number,
  docName: String,
})
const emit = defineEmits(['close-modal'])

// Data
const currentFileExtension = getFileExtension(props.docName)
const { handleSubmit } = useForm({
  validationSchema: {
    attachment(value) {
      if (!value)
        return 'Document is required'

      const fileSize = (value.length * 3) / 4 / 1024 / 1024 // Convert base64 size to MB
      if (fileSize > 1024)
        return 'Document size cannot exceeds 1GB'

      const fileExtension = getFileExtension(editAttachmentDetails.value.attachmentInfo.name)
      return fileExtension == currentFileExtension  ? true : `The updated document is ${currentFileExtension} file format`
    }
  }
})
const loading = ref(false)
const editAttachmentDetails = ref({
  attachmentInfo: null,
  attachment: useField('attachment'),
  extension: null,
})
const acceptedDocInput = ref(
  `application/pdf,
  .doc,.docx,.xml,
  application/msword,
  application/vnd.openxmlformats-officedocument.wordprocessingml.document,
`)

// Methods
function getFileExtension (docName) {
  const extensionIndex = docName.lastIndexOf(".")
  return docName.slice(extensionIndex + 1) // Get the extension
}
const uploadFile = async() => {
  const file = editAttachmentDetails.value.attachmentInfo
  if (file) {
    // Read file as DataURL using a promise-based approach
    const reader = new FileReader()
    reader.readAsDataURL(file)
    try {
      const base64Data = await new Promise((resolve, reject) => {
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
      })
      editAttachmentDetails.value.attachment.value = base64Data.replace(/^.+?;base64,/, '')
      editAttachmentDetails.value.extension = getFileExtension(file.name)
    } catch(e) { ElNotification.error({ message: `Error reading file: ${e}` }) }
  }
  else
  {
    editAttachmentDetails.value.extension = null
    editAttachmentDetails.value.attachment.value = null
  }
}
const updateDoc = handleSubmit(async(values) => {
  loading.value = true
  try {
    const result = await useFetchCustom.$put(`/Repository/${props.docId}`, {
      attachment: values.attachment,
    })
    
    if (!result.error) {
      emit('close-modal', false)
      editAttachmentDetails.value.attachment.resetField()
      editAttachmentDetails.value.attachmentInfo = null
      editAttachmentDetails.value.extension = null
      
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  }
  catch {
    ElNotification.error({ message: "There is a problem with the server. Please try again later." })
  }
  loading.value = false
})
</script>