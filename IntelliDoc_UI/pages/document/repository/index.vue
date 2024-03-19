<template>
  <v-row>
    <v-col cols="12" md="12">
      <UiParentCard title="Repository">
        <v-row class="px-7">
          <!-- Filters -->
          <v-col class="pe-0" cols="3">
            <v-autocomplete
              :items="filterOption.docNameList"
              item-title="name"
              item-value="id"
              placeholder="Documents"
              density="compact"
              variant="outlined"
              v-model="filter.docId"
              hide-details
            />     
          </v-col>
          <v-col class="pe-0" cols="3">
            <v-select
              :items="filterOption.docCategoryList"
              item-title="name"
              item-value="name"
              placeholder="Categories"
              density="compact"
              variant="outlined"
              v-model="filter.category"
              hide-details
            >     
              <template #prepend-item>
                <v-list-item title="All Categories" @click="filter.category = null"/>
              </template>
            </v-select>
          </v-col>
          <v-col>
            <!-- Add New Document -->
            <v-file-input
              class="d-none"
              ref="addDocInput"
              v-model:model-value="addDocInfo.attachmentInfo"
              @update:model-value="addDoc"
              :accept="`application/pdf, .doc,.docx,.xml, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/vnd.ms-excel`"
              hide-details
            />
            <v-btn class="float-end" color="primary" prepend-icon="mdi-file-document-plus-outline" flat @click="addDocInput.click()">New Document</v-btn>
          </v-col>
        </v-row>

        <!-- Document List Table -->
        <div class="pa-7 pt-3 text-body-1">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="headers"
            :items="docList"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr>
                <td><a :href="`/documents/repositories/${item.id}`" target="_blank" class="row-link">{{ item.name }}</a></td>
                <td>{{ item.category }}</td>
                <td>{{ item.modifiedBy }}</td>
                <td>{{ dayjs(item.modifiedDate).format("DD MMM YYYY") }}</td>
                <td>
                  <ul class="m-0 list-inline hstack">
                    <li>
                      <v-tooltip text="Download" activator="parent" location="top" offset="2"/>
                      <v-btn icon="mdi-download-outline" size="small" variant="text" @click="downloadDoc(item.id)"/>
                    </li>
                    <li>
                      <v-tooltip text="Update" activator="parent" location="top" offset="2"/>
                      <v-btn icon="mdi-upload-outline" size="small" variant="text" @click="selectDoc('Update', item)"/>
                    </li>
                    <li>
                      <v-tooltip text="Rename" activator="parent" location="top" offset="2"/>
                      <v-btn icon="mdi-pencil-outline" size="small" variant="text" @click="selectDoc('Rename', item)"/>
                    </li>
                    <li>
                      <v-tooltip text="Archive" activator="parent" location="top" offset="2"/>
                      <el-popconfirm
                        title="Are you sure to archive this document?"
                        icon-color="orange"
                        width="190"
                        @confirm="archiveDoc(item.id)"
                      >
                        <template #reference>
                          <v-btn icon="mdi-archive-outline" size="small" variant="text"/>
                        </template>
                      </el-popconfirm>
                    </li>
                    <li>
                      <v-tooltip text="View Details" activator="parent" location="top" offset="2"/>
                      <v-btn icon="mdi-open-in-new" size="small" variant="text" :href="`/documents/repositories/${item.id}`"/>
                    </li>
                  </ul>
                </td>
              </tr>
            </template>
            <template #bottom>
              <div class="d-flex justify-content-end pt-2">
                <el-pagination
                  layout="total, prev, pager, next"
                  v-model:current-page="currentPage"
                  :page-size="docList.length/pageCount()" 
                  :total="docList.length"
                />
              </div>
            </template>
          </v-data-table>
        </div>
      </UiParentCard>
    </v-col>
  </v-row>

  <!-- Edit Document Information Modal -->
  <SharedUiModal v-model="renameDocModal" title="Rename Document" width="500">
    <DocumentRenameForm
      :doc-id="selectedDocInfo.id"
      :doc-name-old="selectedDocInfo.name"
      @close-modal="(e) => renameDocModal = e"
    />
  </SharedUiModal>

  <!-- Edit Document Attachment Modal -->
  <SharedUiModal v-model="editAttachmentModal" title="Update Document" width="500">
    <DocumentEditAttachmentForm
      :doc-id="selectedDocInfo.id"
      @close-modal="(e) => editAttachmentModal = e"
    />
  </SharedUiModal>
</template>

<script setup>
import { FileDescriptionIcon } from "vue-tabler-icons"
import { Buffer } from 'buffer'
import dayjs from 'dayjs'
import UiParentCard from '@/components/shared/UiParentCard.vue'

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const headers = ref([
  { key: "name", title: "Name" },
  { key: "category", title: "Category" },
  { key: "modifiedBy", title: "Modified By" },
  { key: "modifiedDate", title: "Modified Time" },
  { key: "actions", sortable: false, width: 0 },
])
const filter = ref({
  docId: null,
  category: null,
})
const addDocInfo = ref({
  name: null,
  type: null,
  attachment: null,
  attachmentInfo: null,
})
const selectedDocInfo = ref({
  id: null,
  name: null,
})
const addDocInput = ref(null)
const renameDocModal = ref(false)
const editAttachmentModal = ref(false)
const { data: filterOption } = await fetchData.$get("/Repository/FilterOption")
const { data: docList } = await fetchData.$get("/Repository/Filter", filter.value)

// Head
useHead({
  title: "Repository | USM Document Management System",
})

// Page Meta
definePageMeta({
  breadcrumbsIcon: shallowRef(FileDescriptionIcon),
  breadcrumbs: [
    {
      title: 'Document',
      disabled: false,
    },
    {
      title: 'Repository',
      disabled: false,
    },
  ],
})

// Methods
const pageCount = () => {
  return Math.ceil(docList.value.length / itemsPerPage.value)
}
const getFileType = (docName) => {
  const extensions = {
    "pdf": "PDF",
    "doc": "Word",
    "docx": "Word",
    "xls": "Excel",
    "xlsx": "Excel",
  }
  const extensionIndex = docName.lastIndexOf(".")
  const ext = docName.slice(extensionIndex + 1) // Get the extension
  return extensions[ext] || null // Check for extension in map, return null if not found
}
const addDoc = async() => {
  const file = addDocInfo.value.attachmentInfo[0]
  if (file)
  {
    const reader = new FileReader() // Read file as DataURL using a promise-based approach
    reader.readAsDataURL(file)

    try {
      const base64Data = await new Promise((resolve, reject) => {
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
      })

      addDocInfo.value.attachment = base64Data.replace(/^.+?;base64,/, '')
      addDocInfo.value.name = file.name
      addDocInfo.value.type = getFileType(file.name)

      const fileSize = (addDocInfo.value.attachment.length * 3) / 4 / 1024 // Convert base64 size to KB
      if (fileSize > 28000)
        return ElNotification.warning({ message: "Document size cannot exceeds 28MB" })

      if (getFileType(addDocInfo.value.name) == null)
        return ElNotification.warning({ message: "The document type must be in PDF, Word, or Excel" })

      try {
        const result = await fetchData.$post("/Repository", {
          name: addDocInfo.value.name,
          attachment: addDocInfo.value.attachment,
          type: addDocInfo.value.type,
        })

        if (!result.error) {
          addDocInfo.value.name = null
          addDocInfo.value.type = null
          addDocInfo.value.attachment = null
          addDocInfo.value.attachmentInfo = null
          ElNotification.success({ message: result.message })
          refreshNuxtData()
        }
        else {
          ElNotification.error({ message: result.message })
        }
      } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }

    } catch(e) { ElNotification.error({ message: `Error reading file: ${e}` }) }
  }
  else {
    addDocInfo.value.name = null
    addDocInfo.value.type = null
    addDocInfo.value.attachment = null
    addDocInfo.value.attachmentInfo = null
  }
}
const selectDoc = (action, doc) => {
  selectedDocInfo.value.id = doc.id

  if (action == "Rename") {
    selectedDocInfo.value.name = doc.name
    renameDocModal.value = true
  }
  else if (action == "Update") {
    editAttachmentModal.value = true
  }
  else {
    ElNotification.error({ message: "Undefined action performed" })
  }
}
const downloadDoc = async(docId) => {
  const { data } = await fetchData.$get(`/Repository/GetAttachment/${docId}/Latest`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(data.value.attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[data.value.type] })
  const url = URL.createObjectURL(blob)

  if (data.value.type == 'PDF')
    window.open(url, '_blank')
  else {
    const link = document.createElement('a')
    link.href = url
    link.download = docName
    link.click()
    document.body.removeChild(link)
  }
}
const archiveDoc = async(docId) => {
  try {
    const result = await fetchData.$put(`/Repository/Archive/${docId}`)
    if (!result.error) {
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
</script>