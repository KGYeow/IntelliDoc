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
          <v-row>
            <v-col cols="12" class="py-0">
              <v-checkbox
                class="text-caption"
                color="primary"
                density="compact"
                v-model="hasRelatedDoc"
                @update:model-value="findRelatedDoc($event)"
                hide-details
              >
                <template #label>
                  <v-label class="text-caption">Upload related document</v-label>
                </template>
              </v-checkbox>
            </v-col>
          </v-row>
          <v-row v-if="hasRelatedDoc && addDocInfo.attachment.value">
            <v-col class="pt-0">
              <v-sheet class="p-3" :border="true" rounded>
                <v-row class="justify-content-center">
                  <v-col cols="9" v-if="loading.find">
                    <div class="text-center">
                      Finding related documents from the uploaded document
                      <v-progress-linear class="mt-4" color="primary" rounded indeterminate/>
                    </div>
                  </v-col>
                  <v-col v-else>
                    <v-row class="justify-content-center">
                      <v-col cols="12" v-if="addRelatedDocInfo.length > 0">
                        <v-row class="text-caption font-weight-bold">
                          <v-col cols="3" class="pb-0">
                            <span>Name</span>
                          </v-col>
                          <v-col cols="9" class="pb-0">
                            <span>Document</span>
                          </v-col>
                        </v-row>
                        <v-row class="relatedDocInputs" v-for="(doc, index) in addRelatedDocInfo" :key="index">
                          <v-col cols="3" class="text-caption pb-0">
                            <v-label class="text-caption" v-if="!doc.isAdding">{{ doc.name }}</v-label>
                            <v-text-field
                              variant="outlined"
                              density="compact"
                              v-model="addRelatedDocInfo[index].name"
                              :rules="[value => {
                                if (value == null || value == '')
                                  return 'Document name is required'
                                return true
                              }]"
                              hide-details
                              v-else
                            />
                          </v-col>
                          <v-col cols="9" class="pb-0">
                            <v-file-input
                              variant="outlined"
                              density="compact"
                              prepend-icon=""
                              clear-icon="mdi-close-circle-outline fs-5"
                              :accept="acceptedDocInput"
                              :error-messages="addRelatedDocInfo[index].attachment.errorMessage"
                              v-model:model-value="addRelatedDocInfo[index].attachmentInfo"
                              @update:model-value="uploadRelatedFile(index)"
                              hide-details
                            >
                              <template #append>
                                <v-tooltip text="Add document name" location="top" offset="2" v-if="doc.isAdding">
                                  <template #activator="{ props }">
                                    <v-btn icon="mdi-check" size="x-small" variant="text" type="submit" :="props" @click="addRelatedDocName(index)"/>
                                  </template>
                                </v-tooltip>
                                <v-tooltip text="Delete" location="top" offset="2">
                                  <template #activator="{ props }">
                                    <el-popconfirm
                                      title="Are you sure to delete this document input?"
                                      icon-color="red"
                                      width="220"
                                      :teleported="false"
                                      @confirm="removeRelatedDoc(index)"
                                    >
                                      <template #reference><v-btn icon="mdi-trash-can-outline" size="x-small" variant="text" :="props"/></template>
                                    </el-popconfirm>
                                  </template>
                                </v-tooltip>
                              </template>
                            </v-file-input>
                          </v-col>
                        </v-row>
                      </v-col>
                      <v-col cols="9" class="pb-0" v-if="addRelatedDocInfo.length == 0">
                        <div class="text-center">No related documents found from the uploaded document</div>
                      </v-col>
                      <v-col cols="12" class="relatedDocInputs">
                        <v-btn class="text-muted" color="background" density="compact" prepend-icon="mdi-plus" block flat @click="addRelatedDoc">
                          Add related document
                        </v-btn>
                      </v-col>
                    </v-row>
                  </v-col>
                </v-row>
              </v-sheet>
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
        return 'The document type must be in PDF or Word'

      return true
    },
    relatedAttachment(value) {
      // if (hasRelatedDoc.value) {
      //   if (!value)
      //     return 'Document is required'

      //   const fileSize = (value.length * 3) / 4 / 1024 / 1024 // Convert base64 size to MB
      //   if (fileSize > 1024)
      //     return 'Document size cannot exceeds 1GB'

      //   if (getFileType(addDocInfo.value.name) == null)
      //     return 'The document type must be in PDF or Word'
      // }
      return true
    }
  }
})
const loading = ref({
  create: false,
  find: false
})
const hasRelatedDoc = ref(false)
const addRelatedDocInfo = ref([])
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
  addRelatedDocInfo.value = []
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
const uploadRelatedFile = async(i) => {
  const file = addRelatedDocInfo.value[i].attachmentInfo
  if (file)
  {
    const reader = new FileReader() // Read file as DataURL using a promise-based approach
    reader.readAsDataURL(file)

    try {
      const base64Data = await new Promise((resolve, reject) => {
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
      })

      addRelatedDocInfo.value[i].attachment.value = base64Data.replace(/^.+?;base64,/, '')
      addRelatedDocInfo.value[i].type = getFileType(file.name)
    }
    catch(e) {
      ElNotification.error({ message: `Error reading file: ${e}` })
    }
  }
  else {
    addRelatedDocInfo.value[i].type = null
    addRelatedDocInfo.value[i].attachment.resetField()
    addRelatedDocInfo.value[i].attachmentInfo = null
  }
}
const createDoc = handleSubmit(async(values) => {
  loading.value.create = true
  try {
    const result = await useFetchCustom.$post("/Repository", {
      name: addDocInfo.value.name,
      attachment: values.attachment,
      type: addDocInfo.value.type,
      relatedDoc: addRelatedDocInfo.value.map(doc => ({
        name: doc.name,
        attachment: doc.attachment.value,
        type: doc.type,
      })),
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
      ElNotification.error({ message: result.message })
    }
  }
  catch {
    ElNotification.error({ message: "There is a problem with the server. Please try again later." })
  }
  loading.value.create = false
})
const addRelatedDoc = () => {
  addRelatedDocInfo.value.push({
    isAdding: true,
    name: null,
    type: null,
    attachment: useField('relatedAttachment'), // Create a new field for each document
    attachmentInfo: null,
  })
}
const addRelatedDocName = async(i) => {
  if (addRelatedDocInfo.value[i].name == null || addRelatedDocInfo.value[i].name == '') {
    return
  }
  else {
    addRelatedDocInfo.value[i].isAdding = false
  }
}
const removeRelatedDoc = (index) => {
  addRelatedDocInfo.value.splice(index, 1)
  console.log(addRelatedDocInfo.value)
}
const findRelatedDoc = async(status) => {
  addRelatedDocInfo.value = []
  if (status && addDocInfo.value.attachment.value) {
    loading.value.find = true
    try {
      const result = await useFetchCustom.$put(`/Repository/FindPatterns`, {
        name: addDocInfo.value.name,
        attachment: addDocInfo.value.attachment.value,
      })

      if (!result.error) {
        result.forEach(foundedDoc => {
          addRelatedDocInfo.value.push({
            name: foundedDoc,
            type: null,
            attachment: useField('relatedAttachment'), // Create a new field for each document
            attachmentInfo: null,
          })
        })
        refreshNuxtData()
      }
      else {
        ElNotification.error({ message: result.message })
      }
    }
    catch {
      ElNotification.error({ message: "There is a problem with the server. Please try again later." })
    }
    loading.value.find = false
  }
}
</script>