namespace Gifts_Store.Enums
{
    public enum EmailTemplate
    {
        MembershipApproval,
        MembershipRejection,
        GiftRequestApproval,
        GiftRequestRejection,
        InvoicePDFTemplate,
        InvoiceBody,
        OrderArrival,
    }

    public static class EmailTemplateExtensions
    {
        public static string GetSubject(this EmailTemplate template)
        {
            return template switch
            {
                EmailTemplate.MembershipApproval => "Gift Store Membership Approval",
                EmailTemplate.MembershipRejection => "Gift Store Membership Rejection",
                EmailTemplate.GiftRequestApproval => "Your Gift Request Has Been Approved!",
                EmailTemplate.GiftRequestRejection => "Important Update: Rejection of Your Gift Request",
                EmailTemplate.InvoicePDFTemplate => "Gift Store Invoice.pdf",
                EmailTemplate.InvoiceBody => "Invoice for Your Gift Shop Order",
                EmailTemplate.OrderArrival => "Your Order Has Arrived!",
                _ => string.Empty
            }; ;
        }

        public static string GetBody(this EmailTemplate template, params string[] values)
        {
            string templateString = template switch
            {
                EmailTemplate.MembershipApproval => @"
                Dear {0},<br /><br />

                We are pleased to inform you that your membership request for Gift Store has been accepted! Congratulations and welcome to our community of talented gift makers.

                Your dedication and skills in creating unique and wonderful gifts have impressed us, and we believe you will be a valuable addition to our platform. As an approved gift maker, you now have access to various features and opportunities to showcase your creations to a wider audience.

                Please take a moment to review your account details and ensure all the information is accurate. You can log in to your account using the following credentials:

                <br /><br />Username: {1}
                <br />Email: {2}

                <br /><br />We encourage you to explore the platform, set up your profile, and start uploading your remarkable gift creations. Our community is eagerly waiting to discover your talent!

                If you have any questions or need assistance, please don't hesitate to reach out to our support team. We are here to help you make the most of your Gift Store membership.

                Once again, congratulations on becoming a recognized gift maker on Gift Store. We look forward to seeing your incredible creations and wish you all the success in your endeavors.

                <br /><br />Best regards,

                <br />{3}",

                EmailTemplate.MembershipRejection => @"Dear {0},

                <br /><br />We regret to inform you that your membership request for Gift Store has been rejected. We appreciate your interest in becoming a gift maker on our platform, but after careful consideration, we have decided not to proceed with your application at this time.

                We understand that this may be disappointing news, but please note that the selection process is highly competitive, and we receive a significant number of applications. We encourage you to continue pursuing your passion for creating gifts and exploring other opportunities to showcase your talent.

                Thank you for your understanding, and we wish you all the best in your future endeavors.

                <br /><br />Sincerely,

                <br /><br />{1}
                ",

                EmailTemplate.GiftRequestApproval => @"Dear {0},

                <br /><br />We are pleased to inform you that your gift request has been approved by our gift maker. Your selected gift has passed our quality checks and is ready to be prepared for delivery. However, before we can proceed with the final arrangements, we kindly request you to complete the payment process.

                <br /><br /><b>Gift Details:</b>
                <br /><br />Gift Name: {1}
                <br />Quantity: {2}
                <br />Delivery Address: {3}
                <br />Delivery Date: {4}

                <br /><br />To finalize your order, please follow the steps below:

                <br /><br />1. Visit our website.
                <br />2. Log in to your account using your credentials. If you don't have an account, you can create one quickly.
                <br />3. Navigate to the My Orders section.
                <br />4. Locate your approved gift request and click on the Complete Payment button.
                <br />5. Follow the on-screen instructions to proceed with the payment process.
                <br />6. Once your payment is confirmed, we will initiate the gift preparation.

                <br /><br />We understand the importance of timely delivery and want to ensure a seamless experience for you and the gift recipient. By completing the payment promptly, you help us expedite the process and ensure the gift reaches its destination on time.

                <br /><br />If you have any questions or require assistance with the payment process, please don't hesitate to contact our customer support team. We are available to assist you throughout the journey.

                <br /><br />Thank you for choosing our gift store for this special occasion. We appreciate your trust in our services and look forward to providing you with a delightful gifting experience.

                <br /><br />Best regards,
                <br />{5}",

                EmailTemplate.GiftRequestRejection => @"Dear {0},

                <br /> <br />We regret to inform you that your recent gift request has been rejected by our gift maker. We understand that this may come as a disappointment, and we sincerely apologize for any inconvenience caused.

                <br /> <br /><b>Gift Details:</b>
                <br /> <br />Gift Name: {1}

                <br /> <br />Upon careful review, our gift maker has determined that the requested gift does not meet our quality standards or is currently unavailable. We assure you that we have made every effort to ensure a wide selection of exceptional gifts, but occasionally, certain factors may prevent us from fulfilling all requests.

                <br /><br />We understand the importance of your intention to make this occasion special, and we would be more than happy to assist you in finding an alternative gift that aligns with your preferences. Our customer support team is available to provide personalized recommendations and guide you through the process of selecting another gift from our collection.

                <br /><br />If you would like to explore alternative options or require any further assistance, please do not hesitate to reach out to our customer support team. We are committed to ensuring your satisfaction and will do our best to accommodate your needs.

                <br /><br />Once again, we apologize for any disappointment caused by this rejection. We value your patronage and appreciate your understanding in this matter. We look forward to serving you in the future and providing you with a memorable gifting experience.

                <br /><br />Best regards,
                <br />{2}
                ",

                EmailTemplate.InvoicePDFTemplate => @"
                Order Date: {0}
                Payment Date: {1}
                Sender Name: {2}

                Item: {3}
                Quantity: {4}

                Payment amount: ${5}
                ",

                EmailTemplate.InvoiceBody => @"
                Dear {0},

                <br /><br />Thank you for your recent order from our gift shop. We are pleased to provide you with the invoice for your purchase attached to this emal.

                <br /><br />If you have any questions or concerns regarding the invoice or payment, please don't hesitate to contact our customer support team.

                <br /><br />Thank you for choosing our gift shop. We appreciate your business!

                <br /><br />Best regards,
                <br />{1}
                ",

                EmailTemplate.OrderArrival => @"
                Dear {0},

                <br /><br />We are excited to inform you that your order has arrived! We want to express our gratitude for choosing us for your gifting needs.

                <br /><br />Order Details:
                <br /><br />Order Date: {1}
                <br />Expected Arrival Date: {2}
                <br />Delivery Address: {3}

                <br /><br />Item:{4}
                <br />Quantity: {5}

                <br /><br />Total Amount Paid: {6}

                <br /><br />Your order has been carefully packaged and is ready for delivery. Our dedicated team has ensured that each item is in perfect condition. We strive to provide exceptional service and deliver a delightful experience for our valued customers.

                <br /><br />If you have any questions or need further assistance regarding your order, please feel free to contact our customer support team. We are here to help!

                <br /><br />Thank you once again for choosing us. We hope the arrival of your order brings joy and happiness. We look forward to serving you again in the future.

                <br /><br />Best regards,
                <br />{7}
                ",

                _ => string.Empty
            };

            return string.Format(templateString, values);
        }
    }

}
