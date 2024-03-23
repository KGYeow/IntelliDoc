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
              messages="Document files with a size less than 28MB"
              hide-details="auto"
              show-size
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

      const fileSize = (value.length * 3) / 4 / 1024 // Convert base64 size to KB
      if (fileSize > 28000)
        return 'Document size cannot exceeds 28MB'

      const fileExtension = getFileExtension(editAttachmentDetails.value.attachmentInfo[0].name)
      return fileExtension == currentFileExtension  ? true : `The updated document is ${currentFileExtension} file format`
    }
  }
})
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
  const file = editAttachmentDetails.value.attachmentInfo[0]
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
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>