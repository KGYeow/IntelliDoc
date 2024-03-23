<template>
  <v-row>
    <v-col cols="12" md="12">
      <UiParentCard title="Archive">
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
        </v-row>

        <!-- Archive List Table -->
        <div class="pa-7 pt-3 text-body-1">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="[
              { key: 'name', title: 'Name' },
              { key: 'category', title: 'Category' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :items="archiveList"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr>
                <td><a class="row-link">{{ item.name }}</a></td>
                <td>{{ item.category }}</td>
                <td>
                  <ul class="m-0 list-inline hstack">
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
      </UiParentCard>
    </v-col>
  </v-row>

  <!-- Version History Modal -->
  <SharedUiModal v-model="versionHistoryModal" title="Document Version History" width="600">
    <DocumentArchiveVersionHistory :doc-id="selectedDocInfo.id" @close-modal="(e) => versionHistoryModal = e"/>
  </SharedUiModal>
</template>

<script setup>
import { FileDescriptionIcon } from "vue-tabler-icons"
import UiParentCard from '@/components/shared/UiParentCard.vue'

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const filter = ref({
  docId: null,
  category: null,
})
const selectedDocInfo = ref({
  id: null,
  name: null,
})
const versionHistoryModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/Archive/FilterOption")
const { data: archiveList } = await useFetchCustom.$get("/Archive/Filter", filter.value)

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