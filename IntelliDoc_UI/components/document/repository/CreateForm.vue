<template>
  <form @submit.prevent="createDoc">
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
              :error-messages="addDocInfo.attachment.errorMessage"
              v-model:model-value="addDocInfo.attachmentInfo"
              @update:model-value="uploadFile"
              messages="Document files with a size less than 1GB"
              hide-details="auto"
              show-size
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">
      <v-btn color="primary" type="submit" :disabled="loading">
        <v-progress-circular class="me-2" color="primary" :size="18" :width="3" indeterminate v-if="loading"/>
        {{ loading ? 'Creating' : 'Create' }}
      </v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  validationSchema: {
    attachment(value) {
      if (!value)
        return 'Document is required'

      const fileSize = (value.length * 3) / 4 / 1024 / 1024 // Convert base64 size to MB
      if (fileSize > 1024)
        return 'Document size cannot exceeds 1GB'

      if (getFileType(addDocInfo.value.name) == null)
        return 'The document type must be in PDF or Word'

      return true
    }
  }
})
const loading = ref(false)
const addDocInfo = ref({
  name: null,
  type: null,
  attachment: useField('attachment'),
  attachmentInfo: null,
})
const acceptedDocInput = ref(`
  application/pdf,
  .doc,.docx,.xml,
  application/msword,
  application/vnd.openxmlformats-officedocument.wordprocessingml.document,
`)

// Methods
const getFileExtension = (docName) => {
  const extensionIndex = docName.lastIndexOf(".")
  return docName.slice(extensionIndex + 1) // Get the extension
}
const getFileType = (docName) => {
  const extensions = {
    "pdf": "PDF",
    "doc": "Word",
    "docx": "Word",
    "xls": "Excel",
    "xlsx": "Excel",
  }
  const ext = getFileExtension(docName)
  return extensions[ext] || null // Check for extension in map, return null if not found
}
const uploadFile = async() => {
  const file = addDocInfo.value.attachmentInfo
  if (file)
  {
    const reader = new FileReader() // Read file as DataURL using a promise-based approach
    reader.readAsDataURL(file)

    try {
      const base64Data = await new Promise((resolve, reject) => {
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
      })

      addDocInfo.value.attachment.value = base64Data.replace(/^.+?;base64,/, '')
      addDocInfo.value.name = file.name
      addDocInfo.value.type = getFileType(file.name)

      // if (getFileType(addDocInfo.value.name) == null) {
      //   return ElNotification.warning({ message: "The document type must be in PDF or Word" })
      // }
    }
    catch(e) {
      ElNotification.error({ message: `Error reading file: ${e}` })
    }
  }
  else {
    addDocInfo.value.name = null
    addDocInfo.value.type = null
    addDocInfo.value.attachment.resetField()
    addDocInfo.value.attachmentInfo = null
  }
}
const createDoc = handleSubmit(async(values) => {
  loading.value = true
  try {
    const result = await useFetchCustom.$post("/Repository", {
      name: addDocInfo.value.name,
      attachment: values.attachment,
      type: addDocInfo.value.type,
    })

    if (!result.error) {
      addDocInfo.value.name = null
      addDocInfo.value.type = null
      addDocInfo.value.attachment.resetField()
      addDocInfo.value.attachmentInfo = null
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      console.log(result.message);
      ElNotification.error({ message: result.message })
    }
  }
  catch {
    ElNotification.error({ message: "There is a problem with the server. Please try again later." })
  }
  loading.value = false
})
</script>