<template>
  <el-scrollbar max-height="500px" style="overflow: visible;">
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
              />
            </v-col>
          </v-row>
        </div>
      </v-card-text>
      <v-card-actions class="p-3 justify-content-end">
        <v-btn color="primary" type="submit" :disabled="loading.create">
          <v-progress-circular class="me-2" color="primary" :size="18" :width="3" indeterminate v-if="loading.create"/>
          {{ loading.create ? 'Creating' : 'Create' }}
        </v-btn>
      </v-card-actions>
    </form>
  </el-scrollbar>
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
        return 'The document type must be in PDF'

      return true
    }
  }
})
const loading = ref({
  create: false,
})
const addDocInfo = ref({
  name: null,
  type: null,
  attachment: useField('attachment'),
  attachmentInfo: null,
})
const acceptedDocInput = ref(`
  application/pdf,
`)

// Methods
const getFileExtension = (docName) => {
  const extensionIndex = docName.lastIndexOf(".")
  return docName.slice(extensionIndex + 1) // Get the extension
}
const getFileType = (docName) => {
  const extensions = {
    "pdf": "PDF",
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
  loading.value.create = true
  try {
    const result = await useFetchCustom.$post("/UserManual", {
      name: addDocInfo.value.name,
      attachment: values.attachment,
      type: addDocInfo.value.type,
    })

    if (!result.error) {
      emit('close-modal', false)
      addDocInfo.value.name = null
      addDocInfo.value.type = null
      addDocInfo.value.attachment.resetField()
      addDocInfo.value.attachmentInfo = null
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
  loading.value.create = false
})
</script>