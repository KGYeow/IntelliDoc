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
            <!-- Empty Archive -->
            <v-btn class="float-end" color="primary" prepend-icon="mdi-trash-can-outline" flat @click="emptyArchiveModal = true">Empty Archive</v-btn>
          </v-col>
        </v-row>

        <!-- Archive List Table -->
        <div class="text-body-1 overflow-hidden">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="[
              { key: 'name', title: 'Name' },
              { key: 'category', title: 'Category', minWidth: '150' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :sort-by="[{ key: 'name', order: 'asc' }]"
            sort-desc-icon="mdi-arrow-down-thin"
            sort-asc-icon="mdi-arrow-up-thin"
            :items="archiveList"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr>
                <td style="max-width: 500px;">
                  <v-list-item class="p-0 text-nowrap" :prepend-icon="'text-h5 '+ (item.type == 'PDF' ? 'mdi-file-pdf-box' : 'mdi-file-word-box')">
                    <span>
                      <v-tooltip :text="item.name" activator="parent" location="top" offset="2"/>
                      {{ item.name }}
                    </span>
                  </v-list-item>
                </td>
                <td style="max-width: 250px;">
                  <v-list-item class="p-0 text-nowrap">
                    <span>
                      <v-tooltip :text="item.category" activator="parent" location="top" offset="2"/>
                      {{ item.category }}
                    </span>
                  </v-list-item>
                </td>
                <td>
                  <ul class="m-0 list-inline hstack actions">
                    <li>
                      <v-tooltip text="Version History" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn icon="mdi-history" size="small" variant="text" @click="versionHistoryDoc(item.id)" :="props"/>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-tooltip text="Restore" location="top" offset="2">
                        <template #activator="{ props }">
                          <el-popconfirm
                            title="Are you sure to restore this document?"
                            icon-color="green"
                            width="220"
                            @confirm="restoreDoc(item.id)"
                          >
                            <template #reference><v-btn icon="mdi-restore" size="small" variant="text" :="props"/></template>
                          </el-popconfirm>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-tooltip text="Delete Forever" location="top" offset="2">
                        <template #activator="{ props }">
                          <el-popconfirm
                            title="Are you sure to delete this document forever?"
                            icon-color="red"
                            width="220"
                            @confirm="deleteDoc(item.id)"
                          >
                            <template #reference><v-btn icon="mdi-trash-can-outline" size="small" variant="text" :="props"/></template>
                          </el-popconfirm>
                        </template>
                      </v-tooltip>
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
                  :page-size="archiveList.length/pageCount()"
                  :total="archiveList.length"
                />
              </div>
            </template>
          </v-data-table>
        </div>
      </SharedUiCard>
    </v-col>
  </v-row>

  <!-- Version History Modal -->
  <SharedUiModal v-model="versionHistoryModal" title="Document Version History">
    <DocumentArchiveVersionHistory :doc-id="selectedDocInfo.id" @close-modal="(e) => versionHistoryModal = e"/>
  </SharedUiModal>

  <!-- Empty Archive Confirmation Modal -->
  <SharedUiModal v-model="emptyArchiveModal">
    <DocumentArchiveEmptyAllConfirmation @close-modal="(e) => emptyArchiveModal = e"/>
  </SharedUiModal>
</template>

<script setup>
import { FileDescriptionIcon } from "vue-tabler-icons"

// Data
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
})
const versionHistoryModal = ref(false)
const emptyArchiveModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/Archive/FilterOption")
const { data: archiveList } = await useFetchCustom.$get("/Archive/Filter", filter.value)
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
  title: "Archive | USM Document Management System",
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
      title: 'Archive',
      disabled: false,
    },
  ],
})

// Methods
const pageCount = () => {
  return Math.ceil(archiveList.value.length / itemsPerPage.value)
}
const versionHistoryDoc = (docId) => {
  selectedDocInfo.value.id = docId
  versionHistoryModal.value = true
}
const restoreDoc = async(docId) => {
  try {
    const result = await useFetchCustom.$put(`/Archive/Restore/${docId}/0`)
    if (!result.error) {
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
const deleteDoc = async(docId) => {
  try {
    const result = await useFetchCustom.$delete(`/Archive/Delete/${docId}/0`)
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