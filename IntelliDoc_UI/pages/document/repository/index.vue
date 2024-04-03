<template>
  <v-row>
    <v-col cols="12" md="12">
      <SharedUiCard :header="false" :footer="false">
        <v-row class="pt-4">
          <!-- Filters -->
          <v-col class="pe-0" cols="4">
            <v-autocomplete
              :items="docSearchList"
              item-title="name"
              item-value="id"
              placeholder="Documents"
              density="compact"
              variant="outlined"
              append-inner-icon="mdi-magnify"
              menu-icon=""
              v-model="filter.docId"
              item-props
              hide-details
              :menu-props="{ width: '0'}"
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
          <v-col class="pe-0" cols="2">
            <v-select
              :items="docTypeList"
              item-title="name"
              item-value="name"
              placeholder="File Type"
              density="compact"
              variant="outlined"
              v-model="filter.type"
              hide-details
            >     
              <template #prepend-item>
                <v-list-item title="All Categories" @click="filter.type = null"/>
              </template>
            </v-select>
          </v-col>
        </v-row>
        <v-divider/>
        <v-row>
          <v-col>
            <!-- Add New Document -->
            <v-file-input
              class="d-none"
              ref="addDocInput"
              v-model:model-value="addDocInfo.attachmentInfo"
              @update:model-value="addDoc"
              :accept="`application/pdf, .doc,.docx,.xml, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document`"
              hide-details
            />
            <v-btn class="float-end" color="primary" prepend-icon="mdi-file-document-plus-outline" flat @click="addDocInput.click()">New Document</v-btn>
          </v-col>
        </v-row>

        <!-- Document List Table -->
        <div class="text-body-1 overflow-hidden">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="[
              { key: 'name', title: 'Name' },
              { key: 'category', title: 'Category', minWidth: '150' },
              { key: 'modifiedBy', title: 'Modified By', minWidth: '150' },
              { key: 'modifiedDate', title: 'Modified Time', minWidth: '200' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :sort-by="[{ key: 'name', order: 'asc' }]"
            sort-desc-icon="mdi-arrow-down-thin"
            sort-asc-icon="mdi-arrow-up-thin"
            :items="docList"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr>
                <td style="max-width: 420px;">
                  <v-list-item class="p-0 text-nowrap" :prepend-icon="item.type == 'PDF' ? 'mdi-file-pdf-box fs-5' : 'mdi-file-word-box fs-5'">
                    <span>
                      <v-tooltip :text="item.name" activator="parent" location="top" offset="2"/>
                      {{ item.name }}
                    </span>
                  </v-list-item>
                </td>
                <td style="max-width: 150px;">
                  <v-list-item class="p-0 text-nowrap">
                    <span>
                      <v-tooltip :text="item.category" activator="parent" location="top" offset="2"/>
                      {{ item.category }}
                    </span>
                  </v-list-item>
                </td>
                <td style="max-width: 150px;">
                  <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle fs-5">
                    <span v-if="item.modifiedBy">
                      <v-tooltip :text="item.modifiedBy" activator="parent" location="top" offset="2"/>
                      {{ item.modifiedBy }}
                    </span>
                    <span class="text-muted fst-italic" v-else>
                      <v-tooltip text="Deleted Account" activator="parent" location="top" offset="2"/>
                      Deleted Account
                    </span>
                  </v-list-item>
                </td>
                <td>{{ dayjs(item.modifiedDate).format("DD MMM YYYY, hh:mm A") }}</td>
                <td>
                  <ul class="m-0 list-inline hstack">
                    <li>
                      <v-tooltip text="Download" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn icon="mdi-download-outline" size="small" variant="text" @click="downloadDoc(item)" :="props"/>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-tooltip text="Update" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn icon="mdi-upload-outline" size="small" variant="text" @click="selectDoc('Update', item)" :="props"/>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-tooltip text="Rename" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn icon="mdi-rename-outline" size="small" variant="text" @click="selectDoc('Rename', item)" :="props"/>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-menu width="220" location="left" offset="2">
                        <template #activator="{ props }">
                          <v-btn size="small" variant="text" :="props" icon>
                            <v-icon icon="mdi-dots-vertical"/>
                            <v-tooltip text="More Actions" location="top" offset="2" activator="parent"/>
                          </v-btn>
                        </template>
                        <v-list class="text-body-1" density="compact" elevation="4">
                          <v-list-item prepend-icon="mdi-download-outline" @click="downloadDoc(item)">Download</v-list-item>
                          <v-list-item prepend-icon="mdi-upload-outline" @click="selectDoc('Update', item)">Update</v-list-item>
                          <v-list-item prepend-icon="mdi-rename-outline" @click="selectDoc('Rename', item)">Rename</v-list-item>
                          <v-list-item prepend-icon="mdi-history" @click="selectDoc('VersionHistory', item)">Version History</v-list-item>
                          <v-list-item prepend-icon="mdi-archive-outline" @click="archiveDoc(item.id)">Archive</v-list-item>
                        </v-list>
                      </v-menu>
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
      </SharedUiCard>
    </v-col>
  </v-row>

  <!-- Rename Document Modal -->
  <SharedUiModal v-model="renameDocModal" title="Rename Document" width="500">
    <DocumentRenameForm
      :doc-id="selectedDocInfo.id"
      :doc-name-old="selectedDocInfo.name"
      @close-modal="(e) => renameDocModal = e"
    />
  </SharedUiModal>

  <!-- Update Document Modal -->
  <SharedUiModal v-model="editAttachmentModal" title="Update Document" width="500">
    <DocumentUpdateForm
      :doc-id="selectedDocInfo.id"
      :doc-name="selectedDocInfo.name"
      @close-modal="(e) => editAttachmentModal = e"
    />
  </SharedUiModal>

  <!-- Version History Modal -->
  <SharedUiModal v-model="versionHistoryModal" title="Document Version History" width="700">
    <DocumentRepositoryVersionHistory
      :doc-id="selectedDocInfo.id"
      :doc-name="selectedDocInfo.name"
      :doc-type="selectedDocInfo.type"
      @close-modal="(e) => versionHistoryModal = e"
    />
  </SharedUiModal>
</template>

<script setup>
import { FileDescriptionIcon } from "vue-tabler-icons"
import { Buffer } from 'buffer'
import dayjs from 'dayjs'

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const filter = ref({
  docId: null,
  category: null,
  type: null,
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
  type: null,
})
const addDocInput = ref(null)
const renameDocModal = ref(false)
const editAttachmentModal = ref(false)
const versionHistoryModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/Repository/FilterOption")
const { data: docList } = await useFetchCustom.$get("/Repository/Filter", filter.value)
const docSearchList = filterOption.value.docNameList.map(item => {
  return {
    ...item,
    prependIcon: item.type == "PDF" ? "mdi-file-pdf-box" : "mdi-file-word-box"
  }
})
const docTypeList = ref([
  { name: "PDF", prependIcon: "mdi-file-pdf-box" },
  { name: "Word", prependIcon: "mdi-file-word-box" },
])

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
function getFileExtension (docName) {
  const extensionIndex = docName.lastIndexOf(".")
  return docName.slice(extensionIndex + 1) // Get the extension
}
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
  const ext = getFileExtension(docName)
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
        return ElNotification.warning({ message: "The document type must be in PDF or Word" })
      var testing = addDocInfo.value.attachmentInfo
      console.log(testing);
      try {
        const result = await useFetchCustom.$post("/Repository", {
          name: addDocInfo.value.name,
          attachment: addDocInfo.value.attachment,
          type: addDocInfo.value.type,
        })

        if (!result.error) {
          console.log(result)
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
  selectedDocInfo.value.name = doc.name

  if (action == "Rename") {
    renameDocModal.value = true
  }
  else if (action == "Update") {
    editAttachmentModal.value = true
  }
  else if (action = "VersionHistory") {
    selectedDocInfo.value.type = doc.type
    versionHistoryModal.value = true
  }
  else {
    ElNotification.error({ message: "Undefined action performed" })
  }
}
const downloadDoc = async(doc) => {
  const attachment = await useFetchCustom.$fetch(`/Repository/GetAttachment/${doc.id}/0`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[doc.type] })
  const url = URL.createObjectURL(blob)
  
  if (doc.type == 'PDF')
    window.open(url, '_blank')
  else {
    const link = document.createElement('a')
    link.href = url
    link.download = doc.name
    link.click()
    document.body.appendChild(link)
    document.body.removeChild(link)
  }
}
const archiveDoc = async(docId) => {
  try {
    const result = await useFetchCustom.$put(`/Repository/Archive/${docId}/0`)
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