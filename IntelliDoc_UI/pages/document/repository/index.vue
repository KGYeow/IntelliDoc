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
        <v-divider/>
        <v-row>
          <v-col>
            <!-- Add New Document Button -->
            <v-btn class="float-end" color="primary" :prepend-icon="loading.addDoc ? null : 'mdi-file-document-plus-outline'" flat @click="createDocModal = true" :disabled="loading.addDoc">
              <v-progress-circular class="me-2" color="white" :size="18" :width="3" indeterminate v-if="loading.addDoc"/>
              {{ loading.addDoc ? 'Adding new document' : 'New Document' }}
            </v-btn>
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
                      <v-tooltip :text="item.category" activator="parent" location="top" offset="2" v-if="!isExpanded(internalItem)"/>
                      {{ item.category }}
                    </span>
                  </v-list-item>
                </td>
                <td style="max-width: 150px;">
                  <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle text-h5">
                    <span v-if="item.modifiedBy">
                      <v-tooltip :text="item.modifiedBy" activator="parent" location="top" offset="2" v-if="!isExpanded(internalItem)"/>
                      {{ item.modifiedBy }}
                    </span>
                    <span class="text-muted fst-italic" v-else>
                      <v-tooltip text="Deleted Account" activator="parent" location="top" offset="2" v-if="!isExpanded(internalItem)"/>
                      Deleted Account
                    </span>
                  </v-list-item>
                </td>
                <td>
                  <span>
                    <v-tooltip :text="dayjs(item.modifiedDate).format('DD MMM YYYY, hh:mm A')" activator="parent" location="top" offset="2" v-if="!isExpanded(internalItem)"/>
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
                          <v-list-item prepend-icon="mdi-pencil-outline" @click="selectDoc('Edit', item)">Edit</v-list-item>
                          <v-list-item prepend-icon="mdi-rename-outline" @click="selectDoc('Rename', item)">Rename</v-list-item>
                          <v-list-item
                            :prepend-icon="item.isFlagged ? 'mdi-flag-variant' : 'mdi-flag-variant-outline'"
                            @click="flagDoc(item.id)"
                          >
                            {{ item.isFlagged ? 'Unflag' : 'Flag' }}
                          </v-list-item>
                          <v-divider class="m-2"/>
                          <v-list-item prepend-icon="mdi-file-document-multiple-outline" @click="selectDoc('RelatedDoc', item)">Related Documents</v-list-item>
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
                  <DocumentRepositoryDescriptionInfo
                    :doc-info="item"
                    @open-edit-modal="selectDoc('Edit', item)"
                    @open-related-doc="selectDoc('RelatedDoc', item)"
                  />
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

  <!-- Create Document Modal -->
  <SharedUiModal v-model="createDocModal" title="Create Document" width="700">
    <DocumentRepositoryCreateForm
      :main-doc-id="addRelatedDocInfo.mainDocId"
      @reset-mainDocId="addRelatedDocInfo.mainDocId = $event"
      @close-modal="createDocModal = $event"
    />
  </SharedUiModal>

  <!-- Rename Document Modal -->
  <SharedUiModal v-model="renameDocModal" title="Rename Document" width="500">
    <DocumentRepositoryRenameForm
      :doc-id="selectedDocInfo.id"
      :doc-name-old="selectedDocInfo.name"
      @close-modal="renameDocModal = $event"
    />
  </SharedUiModal>

  <!-- Edit Document Modal -->
  <SharedUiModal v-model="editDocModal" title="Edit Document">
    <DocumentRepositoryEditForm
      :doc="selectedDocInfo"
      :doc-category-list="filterOption.docCategoryList"
      @close-modal="editDocModal = $event"
    />
  </SharedUiModal>

  <!-- Update Document Modal -->
  <SharedUiModal v-model="updateAttachmentModal" title="Update Document" width="500">
    <DocumentRepositoryUpdateForm
      :doc-id="selectedDocInfo.id"
      :doc-name="selectedDocInfo.name"
      @close-modal="updateAttachmentModal = $event"
    />
  </SharedUiModal>

  <!-- Version History Modal -->
  <SharedUiModal v-model="versionHistoryModal" title="Document Version History" width="700">
    <DocumentRepositoryVersionHistory
      :doc-id="selectedDocInfo.id"
      :doc-name="selectedDocInfo.name"
      :doc-type="selectedDocInfo.type"
      @close-modal="versionHistoryModal = $event"
    />
  </SharedUiModal>

  <!-- Related Document Modal -->
  <SharedUiModal v-model="relatedDocModal" title="Related Documents" width="900">
    <DocumentRepositoryRelatedDocument
      :main-doc-id="selectedDocInfo.id"
      @close-modal="relatedDocModal = $event"
      @select-doc="selectDoc($event[0], $event[1])"
      @create-doc="addRelatedDoc($event)"
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
const selectedDocInfo = ref({})
const addRelatedDocInfo = ref({
  mainDocId: null,
})
const loading = ref({
  addDoc: false,
})
const createDocModal = ref(false)
const renameDocModal = ref(false)
const editDocModal = ref(false)
const updateAttachmentModal = ref(false)
const versionHistoryModal = ref(false)
const relatedDocModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/Repository/FilterOption")
const { data: docList, pending: tableLoading } = await useFetchCustom.$get("/Repository/Filter", filter.value)
const docSearchList = computed(() => {
  return filterOption.value.docNameList.map(item => {
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
const selectDoc = (action, doc) => {
  selectedDocInfo.value = doc

  if (action == "Rename") {
    renameDocModal.value = true
  }
  else if (action == "Edit") {
    editDocModal.value = true
  }
  else if (action == "Update") {
    updateAttachmentModal.value = true
  }
  else if (action == "VersionHistory") {
    versionHistoryModal.value = true
  }
  else if (action == "RelatedDoc") {
    relatedDocModal.value = true
  }
  else {
    ElNotification.error({ message: "Undefined action performed" })
  }
}
const addRelatedDoc = (mainDocId) => {
  addRelatedDocInfo.value.mainDocId = mainDocId
  createDocModal.value = true
}
const downloadDoc = async(doc) => {
  loading.value.downloadDoc = true
  const attachment = await useFetchCustom.$fetch(`/Repository/GetAttachment/${doc.id}/0`)
  const mimeType = {
    "PDF": "application/pdf",
    "Word": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "Excel": "application/vnd.ms-excel",
  }
  const arrayBuffer = Buffer.from(attachment, 'base64');
  const blob = new Blob([arrayBuffer], { type: mimeType[doc.type] })
  const url = URL.createObjectURL(blob)

  loading.value.downloadDoc = false
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