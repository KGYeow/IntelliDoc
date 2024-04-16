<template>
  <v-card-text class="px-8 py-4">
    <div class="text-body-1">
      <v-data-table
        density="compact"
        v-model:page="currentPage"
        :headers="[
          { key: 'version', title: 'Version' },
          { key: 'archivedDate', title: 'Archived Time' },
          { key: 'actions', sortable: false, width: 0 },
        ]"
        :sort-by="[{ key: 'version', order: 'desc' }]"
        sort-desc-icon="mdi-arrow-down-thin"
        sort-asc-icon="mdi-arrow-up-thin"
        :items="docVersionHistory"
        :items-per-page="itemsPerPage"
        hover
      >
        <template #item="{ item }">
          <tr>
            <td>{{ item.version }}</td>
            <td>
              <span>
                <v-tooltip :text="dayjs(item.archivedDate).format('DD MMM YYYY, hh:mm A')" activator="parent" location="top" offset="2"/>
                {{ dayjs(item.archivedDate).format("DD MMM YYYY") }}
              </span>
            </td>
            <td>
              <ul class="m-0 list-inline hstack actions">
                <li>
                  <v-tooltip text="Restore" location="top" offset="2">
                    <template #activator="{ props }">
                      <el-popconfirm
                        title="Are you sure to restore this document version?"
                        icon-color="green"
                        width="210"
                        :teleported="false"
                        @confirm="restoreDocVersion(item.version)"
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
                        title="Are you sure to delete this document version forever?"
                        icon-color="red"
                        width="200"
                        :teleported="false"
                        @confirm="deleteDocVersion(item.version)"
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
              :page-size="docVersionHistory.length/pageCount()" 
              :total="docVersionHistory.length"
            />
          </div>
        </template>
      </v-data-table>
    </div>
  </v-card-text>
  <v-card-actions class="p-3 justify-content-end">
    <v-btn color="primary" @click="emit('close-modal', false)">Close</v-btn>
  </v-card-actions>
</template>

<script setup>
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docId: Number,
})
const emit = defineEmits(['close-modal'])

// Data
const currentPage = ref(1)
const itemsPerPage = ref(10)
const { data: docVersionHistory } = await useFetchCustom.$get(`/Archive/VersionHistory/${props.docId}`)

// Methods
const pageCount = () => {
  return Math.ceil(docVersionHistory.value.length / itemsPerPage.value)
}
const restoreDocVersion = async(docVersion) => {
  try {
    const result = await useFetchCustom.$put(`/Archive/Restore/${props.docId}/${docVersion}`)
    if (!result.error) {
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
}
const deleteDocVersion = async(docVersion) => {
  try {
    const result = await useFetchCustom.$delete(`/Archive/Delete/${props.docId}/${docVersion}`)
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