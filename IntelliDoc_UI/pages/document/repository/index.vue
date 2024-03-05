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
                      <v-btn icon="mdi-upload-outline" size="small" variant="text" @click="getEditAttachmentInfo(item.id)"/>
                    </li>
                    <li>
                      <v-tooltip text="Edit Info" activator="parent" location="top" offset="2"/>
                      <v-btn icon="mdi-file-edit-outline" size="small" variant="text" @click="getEditDocInfo(item.id, item.name, item.categoryId, item.caseId)"/>
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

  <!-- Add New Document Button -->
  <el-affix
    class="position-absolute"
    position="bottom"
    :offset="30"
    style="right: 30px; bottom: 100px;"
  >
    <v-tooltip text="Upload Document" activator="parent" location="left" offset="2"/>
    <v-btn icon="mdi-file-document-plus-outline" color="primary" size="large" @click="addDocModal = true"/>
  </el-affix>

  <!-- Add New Document Modal -->
  <SharedUiModal v-model="addDocModal" title="Add New Document" width="500">
    <DocumentCreateForm @close-modal="(e) => addDocModal = e"/>
  </SharedUiModal>

  <!-- Edit Document Information Modal -->
  <SharedUiModal v-model="editDocInfoModal" title="Edit Document Information" width="500">
    <DocumentEditInfoForm
      :edit-document-info="editDocInfoDetails"
      :doc-category-list="filterOption.docCategoryList"
      :case-list="caseList"
      @close-modal="(e) => editDocInfoModal = e"
    />
  </SharedUiModal>

  <!-- Edit Document Attachment Modal -->
  <SharedUiModal v-model="editAttachmentModal" title="Update Document File" width="500">
    <DocumentEditAttachmentForm
      :document-id="editAttachmentInfoId"
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
const editDocInfoDetails = ref({
  docId: null,
  name: null,
  categoryId: null,
  caseId: null,
})
const editAttachmentInfoId = ref(null)
const addDocModal = ref(false)
const editDocInfoModal = ref(false)
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
const getEditDocInfo = (docId, name, categoryId, caseId) => {
  editDocInfoDetails.value.docId = docId
  editDocInfoDetails.value.name = name
  editDocInfoDetails.value.categoryId = categoryId
  editDocInfoDetails.value.caseId = caseId
  editDocInfoModal.value = true
}
const getEditAttachmentInfo = (docId) => {
  editAttachmentInfoId.value = docId
  editAttachmentModal.value = true
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