using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotModel;

namespace Moldovan_Bianca_Lab7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {

        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();

        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            // customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();

            BindDataGrid();

            cmbCustomers.ItemsSource = ctx.Customers.Local; 
            // cmbCustomers.DisplayMemberPath = "FirstName"; 
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local; 
            // cmbInventory.DisplayMemberPath = "Make"; 
            cmbInventory.SelectedValuePath = "CarId";

            //System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        CustId = int.Parse(custIdTextBox.Text.Trim()),
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnDelete.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                custIdTextBox.IsEnabled = false;
            }
            else
                if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnDelete.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                custIdTextBox.IsEnabled = false;

                // phoneNumbersView.Source = queryPhoneNumbers.ToList();
            }
            else
            {
                //if (action == ActionState.Delete)
                //{
                //    try
                //    {
                //        customer = (Customer)customerDataGrid.SelectedItem;
                //        ctx.Customers.Remove(customer);
                //        ctx.SaveChanges();
                //    }
                //    catch (DataException ex)
                //    {
                //        MessageBox.Show(ex.Message);
                //    }
                //    customerViewSource.View.Refresh();

                //    // phoneNumbersView.Source = queryPhoneNumbers.ToList();

                //    btnNew.IsEnabled = true;
                //    btnEdit.IsEnabled = true;
                //    btnDelete.IsEnabled = true;
                //    btnSave.IsEnabled = false;
                //    btnCancel.IsEnabled = false;
                //    //lstPhones.IsEnabled = true;
                //    //btnPrevious.IsEnabled = true;
                //    btnNext.IsEnabled = true;
                //    //txtPhoneNumber.IsEnabled = false;
                //    //txtSubscriber.IsEnabled = false;
                //    //txtPhoneNumber.SetBinding(TextBox.TextProperty, txtPhoneNumberBinding);
                //    //txtSubscriber.SetBinding(TextBox.TextProperty, txtSubscriberBinding);
                //}
            }

            SetValidationBinding();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnSave.IsEnabled = true;
            custIdTextBox.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            custIdTextBox.Clear();
            firstNameTextBox.Clear();
            lastNameTextBox.Clear();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            try
            {
                customer = (Customer)customerDataGrid.SelectedItem;
                ctx.Customers.Remove(customer);
                ctx.SaveChanges();
            }
            catch (DataException ex)
            {
                MessageBox.Show(ex.Message);
            }
            customerViewSource.View.Refresh();

        }








        private void btnSave_inv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    inventory = new Inventory()
                    {
                        CarId = int.Parse(carIdTextBox.Text.Trim()),
                        Make = carMakeTextBox.Text.Trim(),
                        Color = carColorTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_inv.IsEnabled = true;
                btnEdit_inv.IsEnabled = true;
                btnSave_inv.IsEnabled = false;
                btnCancel_inv.IsEnabled = false;
                btnDelete_inv.IsEnabled = true;
                btnPrev_inv.IsEnabled = true;
                btnNext_inv.IsEnabled = true;
                carIdTextBox.IsEnabled = false;
            }
            else
                if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = carMakeTextBox.Text.Trim();
                    inventory.Color = carColorTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);

                btnNew_inv.IsEnabled = true;
                btnEdit_inv.IsEnabled = true;
                btnSave_inv.IsEnabled = false;
                btnCancel_inv.IsEnabled = false;
                btnDelete_inv.IsEnabled = true;
                btnPrev_inv.IsEnabled = true;
                btnNext_inv.IsEnabled = true;
                carIdTextBox.IsEnabled = false;

                // phoneNumbersView.Source = queryPhoneNumbers.ToList();
            }
            else
            {
                //if (action == ActionState.Delete)
                //{
                //    try
                //    {
                //        customer = (Customer)customerDataGrid.SelectedItem;
                //        ctx.Customers.Remove(customer);
                //        ctx.SaveChanges();
                //    }
                //    catch (DataException ex)
                //    {
                //        MessageBox.Show(ex.Message);
                //    }
                //    customerViewSource.View.Refresh();

                //    // phoneNumbersView.Source = queryPhoneNumbers.ToList();

                //    btnNew.IsEnabled = true;
                //    btnEdit.IsEnabled = true;
                //    btnDelete.IsEnabled = true;
                //    btnSave.IsEnabled = false;
                //    btnCancel.IsEnabled = false;
                //    //lstPhones.IsEnabled = true;
                //    //btnPrevious.IsEnabled = true;
                //    btnNext.IsEnabled = true;
                //    //txtPhoneNumber.IsEnabled = false;
                //    //txtSubscriber.IsEnabled = false;
                //    //txtPhoneNumber.SetBinding(TextBox.TextProperty, txtPhoneNumberBinding);
                //    //txtSubscriber.SetBinding(TextBox.TextProperty, txtSubscriberBinding);
                //}
            }
        }

        private void btnNext_inv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_inv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }


        private void btnNew_inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnSave_inv.IsEnabled = true;
            carIdTextBox.IsEnabled = true;
            btnCancel_inv.IsEnabled = true;
        }

        private void btnEdit_inv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnSave_inv.IsEnabled = true;
            btnCancel_inv.IsEnabled = true;
        }

        private void btnCancel_inv_Click(object sender, RoutedEventArgs e)
        {
            btnSave_inv.IsEnabled = false;
            btnCancel_inv.IsEnabled = false;
            carIdTextBox.Clear();
            carMakeTextBox.Clear();
            carColorTextBox.Clear();
        }

        private void btnDelete_inv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            try
            {
                inventory = (Inventory)inventoryDataGrid.SelectedItem;
                ctx.Inventories.Remove(inventory);
                ctx.SaveChanges();
            }
            catch (DataException ex)
            {
                MessageBox.Show(ex.Message);
            }
            inventoryViewSource.View.Refresh();

        }


        private void btnNew_ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnSave_ord.IsEnabled = true;
            btnCancel_ord.IsEnabled = true;
        }

        private void btnEdit_ord_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnSave_ord.IsEnabled = true;
            btnCancel_ord.IsEnabled = true;

         

        }

        private void btnDelete_ord_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            try
            {
                order = (Order)customerOrdersDataGrid.SelectedItem;
                ctx.Orders.Remove(order);
                ctx.SaveChanges();
            }
            catch (DataException ex)
            {
                MessageBox.Show(ex.Message);
            }
            customerOrdersViewSource.View.Refresh();
        }


        private void btnSave_ord_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    Random rnd = new Random();

                    //instantiem Customer entity
                    order = new Order()
                    {
                        OrderId = rnd.Next(1, 1000),
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew_ord.IsEnabled = true;
                btnEdit_ord.IsEnabled = true;
                btnSave_ord.IsEnabled = false;
                btnCancel_ord.IsEnabled = false;
                btnDelete_ord.IsEnabled = true;
                btnPrev_ord.IsEnabled = true;
                btnNext_ord.IsEnabled = true;
            }
            else
                if (action == ActionState.Edit)
            {
                dynamic selectedOrder = customerOrdersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else
            {
                if (action == ActionState.Delete)
                {
                    try
                    {
                        dynamic selectedOrder = customerOrdersDataGrid.SelectedItem;
                        int curr_id = selectedOrder.OrderId;
                        var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);

                        if (deletedOrder != null)
                        {
                            ctx.Orders.Remove(deletedOrder);
                            ctx.SaveChanges();
                            MessageBox.Show("Order Deleted Successfully", "Message");
                            BindDataGrid();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            SetValidationBinding();
        }

        private void btnCancel_ord_Click(object sender, RoutedEventArgs e)
        {
            btnSave_ord.IsEnabled = false;
            btnCancel_ord.IsEnabled = false;
        }

        private void btnNext_ord_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_ord_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }
        
        
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");

            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }



    }
}


