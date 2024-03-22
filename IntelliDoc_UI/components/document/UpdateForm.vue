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
              messages="PDF/word files with a size less than 28MB"
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
})
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  validationSchema: {
    attachment(value) {
      if (!value)
        return 'Document is required'

      const fileSize = (value.length * 3) / 4 / 1024 // Convert base64 size to KB
      if (fileSize > 28000)
        return 'Document size cannot exceeds 28MB'

      const fileType = getFileType(editAttachmentDetails.value.attachmentInfo[0].name)
      return fileType ? true : 'The document type must be in PDF or Word'
    }
  }
})
const editAttachmentDetails = ref({
  attachmentInfo: null,
  attachment: useField('attachment'),
  type: null,
})
const acceptedDocInput = ref(
  `application/pdf,
  .doc,.docx,.xml,
  application/msword,
  application/vnd.openxmlformats-officedocument.wordprocessingml.document,
`)

// Methods
const getFileType = (docName) => {
  const extensions = {
    "pdf": "PDF",
    "doc": "Word",
    "docx": "Word",
    // "xls": "Excel",
    // "xlsx": "Excel",
  }
  const extensionIndex = docName.lastIndexOf(".")
  const ext = docName.slice(extensionIndex + 1) // Get the extension
  return extensions[ext] || null // Check for extension in map, return null if not found
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
      editAttachmentDetails.value.type = getFileType(file.name)
    } catch(e) { ElNotification.error({ message: `Error reading file: ${e}` }) }
  }
  else
  {
    editAttachmentDetails.value.type = null
    editAttachmentDetails.value.attachment.value = null
  }
}
const updateDoc = handleSubmit(async(values) => {
  try {
    const result = await fetchData.$put(`/Repository/${props.docId}`, {
      attachment: values.attachment,
      type: editAttachmentDetails.value.type,
    })
    
    if (!result.error) {
      emit('close-modal', false)
      editAttachmentDetails.value.attachment.resetField()
      editAttachmentDetails.value.type = null
      editAttachmentDetails.value.attachmentInfo = null
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>