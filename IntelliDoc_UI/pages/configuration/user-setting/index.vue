<template>
  <v-row>
    <v-col cols="12" md="12">
      <SharedUiCard :header="false" :footer="false">
        <v-row class="pt-4">
          <!-- Filters -->
          <v-col class="pe-0" cols="6">
            <v-autocomplete
              :items="userFullNameSearchList"
              item-title="fullName"
              item-value="id"
              placeholder="Full Name"
              density="compact"
              variant="outlined"
              append-inner-icon="mdi-magnify"
              menu-icon=""
              v-model="filter.userId"
              item-props
              hide-details
              :menu-props="{ width: '0'}"
            />
          </v-col>
        </v-row>
        <v-divider/>
        <v-row>
          <v-col>
            <!-- Add New User -->
            <v-btn class="float-end" color="primary" prepend-icon="mdi-account-plus-outline" flat @click="addUserModal = true">New User</v-btn>
          </v-col>
        </v-row>

        <!-- User List Table -->
        <div class="text-body-1 overflow-hidden">
          <v-data-table
            density="comfortable"
            v-model:page="currentPage"
            :headers="[
              { key: 'fullName', title: 'Full Name' },
              { key: 'email', title: 'Email' },
              { key: 'isActive', title: 'Status', sortable: false , minWidth: '100' },
              { key: 'role', title: 'Role', minWidth: '100' },
              { key: 'actions', sortable: false, width: 0 },
            ]"
            :sort-by="[{ key: 'fullName', order: 'asc' }]"
            sort-desc-icon="mdi-arrow-down-thin"
            sort-asc-icon="mdi-arrow-up-thin"
            :items="userList"
            :items-per-page="itemsPerPage"
            hover
          >
            <template #item="{ item }">
              <tr>
                <td style="max-width: 450px;">
                  <v-list-item class="p-0 text-nowrap" prepend-icon="mdi-account-circle">{{ item.fullName }}</v-list-item>
                </td>
                <td style="max-width: 250px;">
                  <v-list-item class="p-0 text-nowrap">{{ item.email }}</v-list-item>
                </td>
                <td>
                  <el-tag :type="item.isActive ? 'success' : 'danger'" effect="light">
                    {{ item.isActive ? 'Active' : 'Inactive' }}
                  </el-tag>
                </td>
                <td>{{ item.role }}</td>
                <td>
                  <ul class="m-0 list-inline hstack actions">
                    <li>
                      <v-tooltip text="Edit" location="top" offset="2">
                        <template #activator="{ props }">
                          <v-btn icon="mdi-rename-outline" size="small" variant="text" @click="selectUser('Edit', item)" :="props"/>
                        </template>
                      </v-tooltip>
                    </li>
                    <li v-if="item.isActive">
                      <v-tooltip text="Inactivate" location="top" offset="2">
                        <template #activator="{ props }">
                          <el-popconfirm
                            title="Are you sure to inactivate this user account?"
                            icon-color="orange"
                            width="210"
                            @confirm="activationUser(item.id, false)"
                          >
                            <template #reference><v-btn icon="mdi-account-lock-outline" size="small" variant="text" :="props" :disabled="item.id == user.id"/></template>
                          </el-popconfirm>
                        </template>
                      </v-tooltip>
                    </li>
                    <li v-else>
                      <v-tooltip text="Activate" location="top" offset="2">
                        <template #activator="{ props }">
                          <el-popconfirm
                            title="Are you sure to activate this user account?"
                            icon-color="orange"
                            width="200"
                            @confirm="activationUser(item.id, true)"
                          >
                            <template #reference><v-btn icon="mdi-account-reactivate-outline" size="small" variant="text" :="props"/></template>
                          </el-popconfirm>
                        </template>
                      </v-tooltip>
                    </li>
                    <li>
                      <v-tooltip text="Delete Forever" location="top" offset="2">
                        <template #activator="{ props }">
                          <el-popconfirm
                            title="Are you sure to delete this user account forever?"
                            icon-color="red"
                            width="220"
                            @confirm="selectUser('Delete', item)"
                          >
                            <template #reference><v-btn icon="mdi-trash-can-outline" size="small" variant="text" :="props" :disabled="item.id == user.id"/></template>
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
                  :page-size="userList.length/pageCount()" 
                  :total="userList.length"
                />
              </div>
            </template>
          </v-data-table>
        </div>
      </SharedUiCard>
    </v-col>
  </v-row>

  <!-- Add User Modal -->
  <SharedUiModal v-model="addUserModal" title="Add User Account" width="700">
    <UserCreateForm @close-modal="(e) => addUserModal = e"/>
  </SharedUiModal>

  <!-- Edit User Modal -->
  <SharedUiModal v-model="editUserModal" title="Edit User Account" width="500">
    <UserEditForm :user-info="selectedUserInfo" @close-modal="(e) => editUserModal = e"/>
  </SharedUiModal>

  <!-- Delete User Confirmation Modal -->
  <SharedUiModal v-model="deleteUserModal" title="Delete User Account Confirmation" width="500">
    <DeleteConfirmationForm :user-id="selectedUserInfo.id" @close-modal="(e) => deleteUserModal = e"/>
  </SharedUiModal>
</template>

<script setup>
import { SettingsIcon } from "vue-tabler-icons"
import DeleteConfirmationForm from "~/components/user/DeleteConfirmationForm.vue";

// Data
const { data: user } = useAuth()
const currentPage = ref(1)
const itemsPerPage = ref(10)
const filter = ref({
  userId: null,
})
const selectedUserInfo = ref({})
const addUserModal = ref(false)
const editUserModal = ref(false)
const deleteUserModal = ref(false)
const { data: filterOption } = await useFetchCustom.$get("/User/FilterOption")
const { data: userList } = await useFetchCustom.$get("/User/Filter", filter.value)
const userFullNameSearchList = filterOption.value.map(item => {
  return {
    ...item,
    prependIcon: "mdi-account-circle"
  }
})

// Head
useHead({
  title: "User Setting | USM Document Management System",
})

// Page Meta
definePageMeta({
  breadcrumbsIcon: shallowRef(SettingsIcon),
  breadcrumbs: [
    {
      title: 'Configuration',
      disabled: false,
    },
    {
      title: 'User Setting',
      disabled: false,
    },
  ],
})

// Methods
const pageCount = () => {
  return Math.ceil(userList.value.length / itemsPerPage.value)
}
const selectUser = (action, user) => {
  selectedUserInfo.value = user

  if (action == "Edit") {
    editUserModal.value = true
  }
  else if (action == "Delete") {
    deleteUserModal.value = true
  }
  else {
    ElNotification.error({ message: "Undefined action performed" })
  }
}
const activationUser = async(userId, isActivate) => {
  try {
    const result = await useFetchCustom.$put(`/User/Activation/${userId}/${isActivate}`)
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