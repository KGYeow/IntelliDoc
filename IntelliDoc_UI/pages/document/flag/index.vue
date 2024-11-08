<template>
  <v-row>
    <v-col cols="12" md="12">
      <SharedUiCard :header="false" :footer="false">
        <v-row class="pt-4">
          <!-- Filters -->
          <v-col class="pe-0" cols="5">
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
              @update:search="docSearchBar.query = $event"
              :custom-filter="docSearchFilter"
              auto-select-first
              item-props
              hide-details
              :menu-props="{ width: '0' }"
            >
              <template #no-data>
                <v-list-item>
                  <v-list-item-title>
                    No results matching "<strong>{{ docSearchBar.query }}</strong>"
                  </v-list-item-title>
                </v-list-item>
              </template>
              <template #item="{ props }">
                <v-list-item :="props">
                  <template #append>
                    <v-btn density="compact" variant="plain" icon v-if="props.relatedDocs.length > 0">
                      <v-icon icon="mdi-chevron-right-circle"/>
                      <v-menu activator="parent" location="end" width="350px" offset="15">
                        <v-list :items="props.relatedDocs" item-title="name">
                          <v-list-subheader class="fw-bold bg-grey200">Relevant Documents</v-list-subheader>
                          <v-list-item
                            :prepend-icon="subitem.type == 'PDF' ? 'mdi-file-pdf-box' : 'mdi-file-word-box'"
                            v-for="subitem in props.relatedDocs"
                          >
                            <v-list-item-title class="text-caption">{{ subitem.name }}</v-list-item-title>
                          </v-list-item>
                        </v-list>
                      </v-menu>
                    </v-btn>
                  </template>
                </v-list-item>
              </template>
            </v-autocomplete>
          </v-col>
          <v-col class="pe-0" cols="2">
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
                <v-divider class="m-2"/>
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
              item-props
              hide-details
            >     
              <template #prepend-item>
                <v-list-item title="All Categories" @click="filter.type = null"/>
                <v-divider class="m-2"/>
              </template>
            </v-select>
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
              { key: 'modifiedDate', title: 'Modified Time', width: '150' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :sort-by="[{ key: 'name', order: 'asc' }]"
            sort-desc-icon="mdi-arrow-down-thin"
            sort-asc-icon="mdi-arrow-up-thin"
            :items="docList"
            :items-per-page="itemsPerPage"
            :loading="tableLoading"
            hover
          >
            <template #item="{ item, internalItem, toggleExpand, isExpanded }">
              <tr :class="{ 'bg-background': isExpanded(internalItem) }">
                <td style="max-width: 420px;">
                  <v-list-item
                    class="p-0 text-nowrap"
                    :prepend-icon="'text-h5 '+ (item.type == 'PDF' ? 'mdi-file-pdf-box' : 'mdi-file-word-box')"
                    :append-icon="item.isFlagged ? 'mdi-flag-variant text-h5' : null"
                  >
                    <span class="row-link" @click="toggleExpand(internalItem)">
                      <v-tooltip :text="item.name" activator="parent" location="top" offset="2" v-if="!isExpanded(internalItem)"/>
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
                  <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle text-h5">
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
                <td>
                  <span>
                    <v-tooltip :text="dayjs(item.modifiedDate).format('DD MMM YYYY, hh:mm A')" activator="parent" location="top" offset="2"/>
                    {{ dayjs(item.modifiedDate).format("DD MMM YYYY") }}
                  </span>
                </td>
                <td>
                  <ul class="m-0 list-inline hstack actions">
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
                      <v-tooltip text="Flag" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn
                            :icon="item.isFlagged ? 'mdi-flag-variant' : 'mdi-flag-variant-outline'"
                            size="small"
                            variant="text"
                            @click="flagDoc(item.id)"
                            :="props"
                          />
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
                          <v-list-item
                            :prepend-icon="item.isFlagged ? 'mdi-flag-variant' : 'mdi-flag-variant-outline'"
                            @click="flagDoc(item.id)"
                          >
                            {{ item.isFlagged ? 'Unflag' : 'Flag' }}
                          </v-list-item>
                          <v-list-item prepend-icon="mdi-history" @click="selectDoc('VersionHistory', item)">Version History</v-list-item>
                          <v-divider class="m-2"/>
                          <v-list-item prepend-icon="mdi-archive-outline" @click="archiveDoc(item.id)">Archive</v-list-item>
                        </v-list>
                      </v-menu>
                    </li>
                  </ul>
                </td>
              </tr>
            </template>
            <template #expanded-row="{ columns, item }">
              <tr class="expanded">
                <td :colspan="columns.length">
                  <DocumentRepositoryDescriptionInfo :doc-info="item"/>
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
    <DocumentRepositoryRenameForm
      :doc-id="selectedDocInfo.id"
      :doc-name-old="selectedDocInfo.name"
      @close-modal="(e) => renameDocModal = e"
    />
  </SharedUiModal>

  <!-- Update Document Modal -->
  <SharedUiModal v-model="editAttachmentModal" title="Update Document" width="500">
    <DocumentRepositoryUpdateForm
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
const docSearch = ref(null)
const currentPage = ref(1)
const itemsPerPage = ref(10)
const filter = ref({
  docId: null,
  category: null,
  type: null,
})
const selectedDocInfo = ref({
  id: null,
  name: null,
  type: null,
})
const renameDocModal = ref(false)
const editAttachmentModal = ref(false)
const versionHistoryModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/Flag/FilterOption")
const { data: docList, pending: tableLoading } = await useFetchCustom.$get("/Flag/Filter", filter.value)
const docSearchList = computed(() => {
  return filterOption.value.fullDocNameList.map(item => {
    return {
      ...item,
      prependIcon: item.type == "PDF" ? "mdi-file-pdf-box" : "mdi-file-word-box"
    }
  })
})
const docTypeList = ref([
  { name: "PDF", prependIcon: "mdi-file-pdf-box" },
  { name: "Word", prependIcon: "mdi-file-word-box" },
])

// Head
useHead({
  title: "Flagged | USM Document Management System",
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
      title: 'Flagged',
      disabled: false,
    },
  ],
})

// Methods
const docSearchFilter = (itemTitle, queryText, item) => {
  const comparedDocName = itemTitle.toLowerCase()
  const comparedRelatedDocName = item.raw.relatedDocs
  const searchText = queryText.toLowerCase()
  const result = comparedDocName.indexOf(searchText)
  return result > -1 ? result : comparedRelatedDocName.some(doc => { return doc.name.toLowerCase().indexOf(searchText) > -1 })
}
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
const flagDoc = async(docId) => {
  try {
    const result = await useFetchCustom.$put(`/Flag/${docId}`)
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